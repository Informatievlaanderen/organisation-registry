/**
 * GET /callback
 * Keycloak redirect-handler:
 * 1. Ruilt de authorization code in voor tokens (authorization_code grant)
 * 2. Doet een RFC 8693 token exchange: nuxt-bff token → organisation-registry-api token
 * 3. Slaat beide tokens op in de encrypted session cookie
 */
import { defineEventHandler, getQuery, sendRedirect } from 'h3'
import { getSession, saveSession, clearSession } from '../utils/session'

export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()
  const query = getQuery(event)
  const session = getSession(event, config.sessionSecret)

  const code = query.code as string | undefined
  const state = query.state as string | undefined
  const error = query.error as string | undefined

  if (error) {
    clearSession(event)
    return sendRedirect(event, `/?error=${encodeURIComponent(String(query.error_description || error))}`)
  }

  if (!code || !state || state !== session.state || !session.codeVerifier) {
    clearSession(event)
    return sendRedirect(event, `/?error=invalid_state`)
  }

  const keycloakTokenUrl = `${config.keycloakInternalUrl}/realms/${config.public.keycloakRealm}/protocol/openid-connect/token`

  // Stap 1: wissel de authorization code in bij Keycloak (authorization_code grant)
  const tokenRes = await $fetch<any>(keycloakTokenUrl, {
    method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    body: new URLSearchParams({
      grant_type: 'authorization_code',
      client_id: config.public.keycloakClientId,
      client_secret: config.keycloakClientSecret,
      redirect_uri: `${config.public.appBaseUrl}/callback`,
      code,
      code_verifier: session.codeVerifier,
    }).toString(),
  }).catch((err: any) => {
    throw createError({ statusCode: 502, statusMessage: `Keycloak code exchange failed: ${err.message}` })
  })

  const accessToken = tokenRes.access_token as string

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

  saveSession(event, {
    accessToken,
    exchangedToken,
    exchangeError,
    codeVerifier: undefined,
    state: undefined,
  }, config.sessionSecret)

  return sendRedirect(event, '/')
})
