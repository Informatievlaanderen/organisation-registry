/**
 * GET /api/logout
 * Wist de sessie en redirect naar Keycloak logout.
 * id_token_hint is optioneel — realm heeft oidc.logout.idtoken.required=false.
 */
import { defineEventHandler, sendRedirect } from 'h3'
import { clearSession } from '../utils/session'

export default defineEventHandler((event) => {
  const config = useRuntimeConfig()
  clearSession(event)

  const params = new URLSearchParams({
    client_id: config.public.keycloakClientId,
    post_logout_redirect_uri: config.public.appBaseUrl,
  })

  const logoutUrl = `${config.public.keycloakUrl}/realms/${config.public.keycloakRealm}/protocol/openid-connect/logout?${params}`
  return sendRedirect(event, logoutUrl)
})
