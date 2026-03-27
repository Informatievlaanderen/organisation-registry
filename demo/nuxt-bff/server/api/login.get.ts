/**
 * GET /api/login
 * Start de Keycloak authorization code + PKCE flow.
 * 
 * Omdat cookies niet betrouwbaar zijn tijdens cross-site redirects (SameSite restricties),
 * encoderen we de codeVerifier in de state parameter zelf (encrypted).
 * Format: base64url(encrypt(JSON.stringify({ nonce, codeVerifier })))
 */
import { defineEventHandler, sendRedirect } from 'h3'
import { randomBytes, createHash, createCipheriv } from 'crypto'

function base64url(buf: Buffer): string {
  return buf.toString('base64').replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '')
}

function base64urlToBuffer(str: string): Buffer {
  // Restore base64 padding
  const pad = (4 - (str.length % 4)) % 4
  const b64 = str.replace(/-/g, '+').replace(/_/g, '/') + '='.repeat(pad)
  return Buffer.from(b64, 'base64')
}

function getKey(secret: string): Buffer {
  const buf = Buffer.alloc(32, 0)
  Buffer.from(secret).copy(buf, 0, 0, 32)
  return buf
}

function encryptState(data: { nonce: string; codeVerifier: string }, secret: string): string {
  const iv = randomBytes(16)
  const key = getKey(secret)
  const cipher = createCipheriv('aes-256-cbc', key, iv)
  const json = JSON.stringify(data)
  const encrypted = Buffer.concat([cipher.update(json, 'utf8'), cipher.final()])
  // Combine IV + encrypted data and base64url encode
  const combined = Buffer.concat([iv, encrypted])
  return base64url(combined)
}

export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()

  const nonce = base64url(randomBytes(16))
  const codeVerifier = base64url(randomBytes(32))
  const codeChallenge = base64url(
    createHash('sha256').update(codeVerifier).digest()
  )

  // Encrypt nonce + codeVerifier into state parameter
  const state = encryptState({ nonce, codeVerifier }, config.sessionSecret)

  const params = new URLSearchParams({
    response_type: 'code',
    client_id: config.public.keycloakClientId,
    redirect_uri: `${config.public.appBaseUrl}/callback`,
    scope: 'openid profile dv_organisatieregister_cjmbeheerder',
    state,
    code_challenge: codeChallenge,
    code_challenge_method: 'S256',
    prompt: 'select_account',
  })

  const authUrl = `${config.public.keycloakUrl}/realms/${config.public.keycloakRealm}/protocol/openid-connect/auth?${params}`
  return sendRedirect(event, authUrl)
})
