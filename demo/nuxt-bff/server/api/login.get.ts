/**
 * GET /api/login
 * Start de Keycloak authorization code + PKCE flow.
 * Genereert state + code_verifier, slaat ze op in sessie, redirect naar Keycloak.
 */
import { defineEventHandler, sendRedirect, getQuery } from 'h3'
import { randomBytes, createHash } from 'crypto'
import { saveSession } from '../utils/session'

function base64url(buf: Buffer): string {
  return buf.toString('base64').replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '')
}

export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()

  const state = base64url(randomBytes(16))
  const codeVerifier = base64url(randomBytes(32))
  const codeChallenge = base64url(
    createHash('sha256').update(codeVerifier).digest()
  )

  saveSession(event, { state, codeVerifier }, config.sessionSecret)

  const params = new URLSearchParams({
    response_type: 'code',
    client_id: config.public.keycloakClientId,
    redirect_uri: `${config.public.appBaseUrl}/callback`,
    scope: 'openid profile dv_organisatieregister_cjmbeheerder',
    state,
    code_challenge: codeChallenge,
    code_challenge_method: 'S256',
  })

  const authUrl = `${config.public.keycloakUrl}/realms/${config.public.keycloakRealm}/protocol/openid-connect/auth?${params}`
  return sendRedirect(event, authUrl)
})
