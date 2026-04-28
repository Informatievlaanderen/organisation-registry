/**
 * GET /callback
 * Keycloak redirect-handler:
 * 1. Decrypt de state parameter om de codeVerifier te krijgen
 * 2. Ruilt de authorization code in voor tokens (authorization_code grant)
 * 3. Doet een RFC 8693 token exchange: nuxt-bff token → organisation-registry-api token
 * 4. Slaat beide tokens op in de encrypted session cookie
 */
import { defineEventHandler, getQuery, sendRedirect } from 'h3'
import { saveSession, clearSession } from '../utils/session'
import { getAppBaseUrl } from '../utils/urls'
import { createDecipheriv } from 'crypto'

function base64urlToBuffer(str: string): Buffer {
  const pad = (4 - (str.length % 4)) % 4
  const b64 = str.replace(/-/g, '+').replace(/_/g, '/') + '='.repeat(pad)
  return Buffer.from(b64, 'base64')
}

function getKey(secret: string): Buffer {
  const buf = Buffer.alloc(32, 0)
  Buffer.from(secret).copy(buf, 0, 0, 32)
  return buf
}

function decryptState(state: string, secret: string): { nonce: string; codeVerifier: string } | null {
  try {
    const combined = base64urlToBuffer(state)
    const iv = combined.subarray(0, 16)
    const encrypted = combined.subarray(16)
    const key = getKey(secret)
    const decipher = createDecipheriv('aes-256-cbc', key, iv)
    const decrypted = Buffer.concat([decipher.update(encrypted), decipher.final()])
    return JSON.parse(decrypted.toString('utf8'))
  } catch {
    return null
  }
}

export default defineEventHandler(async (event) => {
  console.log('[callback] Handler started')
  const config = useRuntimeConfig()
  const query = getQuery(event)
  console.log('[callback] Query params:', { code: query.code ? 'present' : 'missing', state: query.state ? 'present' : 'missing', error: query.error })

  const code = query.code as string | undefined
  const state = query.state as string | undefined
  const error = query.error as string | undefined

  if (error) {
    clearSession(event)
    return sendRedirect(event, `/?error=${encodeURIComponent(String(query.error_description || error))}`)
  }

  if (!code || !state) {
    clearSession(event)
    return sendRedirect(event, `/?error=missing_code_or_state`)
  }

  // Decrypt state to get codeVerifier
  const stateData = decryptState(state, config.sessionSecret)
  if (!stateData || !stateData.codeVerifier) {
    clearSession(event)
    return sendRedirect(event, `/?error=invalid_state`)
  }

  const { codeVerifier } = stateData

  const keycloakTokenUrl = `${config.keycloakInternalUrl}/realms/${config.public.keycloakRealm}/protocol/openid-connect/token`

  const appBaseUrl = getAppBaseUrl(event)

  // Stap 1: wissel de authorization code in bij Keycloak (authorization_code grant)
  const tokenRes = await $fetch<any>(keycloakTokenUrl, {
    method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    body: new URLSearchParams({
      grant_type: 'authorization_code',
      client_id: config.public.keycloakClientId,
      client_secret: config.keycloakClientSecret,
      redirect_uri: `${appBaseUrl}/callback`,
      code,
      code_verifier: codeVerifier,
    }).toString(),
  }).catch((err: any) => {
    throw createError({ statusCode: 502, statusMessage: `Keycloak code exchange failed: ${err.message}` })
  })

  const accessToken = tokenRes.access_token as string
  const idToken = tokenRes.id_token as string | undefined

  console.log('accessToken', accessToken)
  console.log('idToken', idToken)

  // Stap 2: RFC 8693 token exchange
  // nuxt-bff wisselt zijn eigen access token in voor een token gericht op organisation-registry-api.
  // Keycloak 26 standard token exchange (V2) is standaard ingebouwd — geen extra flags nodig.
  // Vereiste: nuxt-bff client heeft token.exchange.standard.enabled=true in realm-export.json.
  let exchangedToken: string | undefined
  let exchangeError: string | undefined

  try {
    const exchangeRes = await $fetch<any>(keycloakTokenUrl, {
      method: 'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body: new URLSearchParams({
        grant_type: 'urn:ietf:params:oauth:grant-type:token-exchange',
        client_id: config.public.keycloakClientId,
        client_secret: config.keycloakClientSecret,
        subject_token: accessToken,
        subject_token_type: 'urn:ietf:params:oauth:token-type:access_token',
        requested_token_type: 'urn:ietf:params:oauth:token-type:access_token',
        audience: 'organisation-registry-api',
      }).toString(),
    })
    exchangedToken = exchangeRes.access_token as string
  } catch (err: any) {
    // Exchange mislukt: toon dit in de UI maar blokkeer login niet
    const errData = err?.data ?? err?.response?._data
    exchangeError = errData?.error_description ?? errData?.error ?? err?.message ?? 'token exchange failed'
  }

  console.log('[callback] Saving session with tokens:', {
    hasAccessToken: !!accessToken,
    hasExchangedToken: !!exchangedToken,
    exchangeError
  })

  saveSession(event, {
    accessToken,
    idToken,
    exchangedToken,
    exchangeError,
    codeVerifier: undefined,
    state: undefined,
  }, config.sessionSecret)

  console.log('[callback] Session saved, redirecting to /')
  return sendRedirect(event, '/')
})
