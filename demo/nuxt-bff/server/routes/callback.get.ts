/**
 * GET /callback
 * Keycloak redirect-handler. Ruilt de authorization code in voor tokens,
 * doet vervolgens exchange bij de Organisation Registry API voor een custom JWT,
 * en slaat beide op in de encrypted session cookie.
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
    return sendRedirect(event, '/?error=invalid_state')
  }

  // Stap 1: wissel de authorization code in bij Keycloak (server-side)
  const keycloakTokenUrl = `${config.keycloakInternalUrl}/realms/${config.public.keycloakRealm}/protocol/openid-connect/token`
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
    throw createError({ statusCode: 502, statusMessage: `Keycloak token exchange failed: ${err.message}` })
  })

  const accessToken = tokenRes.access_token as string

  // Stap 2: wissel het Keycloak access token in bij de Organisation Registry API
  // voor een custom JWT (de exchange endpoint verwacht de authorization code flow,
  // maar wij gebruiken hier het access token direct als Bearer voor de security endpoint)
  //
  // BELANGRIJK: dit is de demonstratie van het probleem.
  // /v1/security/exchange is bedoeld voor de Angular SPA die een code + verifier stuurt.
  // Hier proberen we het Keycloak access token te gebruiken als Bearer op een
  // [OrganisationRegistryAuthorize] endpoint — dat werkt NIET omdat dat scheme
  // alleen het custom JWT accepteert.
  //
  // Dus: customJwt = undefined, en we tonen dit probleem in de UI.

  saveSession(event, {
    accessToken,
    customJwt: undefined, // bewust leeg — exchange niet mogelijk via BFF flow
    codeVerifier: undefined,
    state: undefined,
  }, config.sessionSecret)

  return sendRedirect(event, '/')
})
