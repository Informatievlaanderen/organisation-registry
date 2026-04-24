/**
 * GET /api/logout
 * Wist de sessie en redirect naar Keycloak logout.
 * id_token_hint is optioneel — realm heeft oidc.logout.idtoken.required=false.
 */
import { defineEventHandler, sendRedirect } from 'h3'
import { clearSession } from '../utils/session'
import { getAppBaseUrl, getKeycloakUrl } from '../utils/urls'

export default defineEventHandler((event) => {
  const config = useRuntimeConfig()
  clearSession(event)

  const appBaseUrl = getAppBaseUrl(event)
  const keycloakUrl = getKeycloakUrl(event)

  const params = new URLSearchParams({
    client_id: config.public.keycloakClientId,
    post_logout_redirect_uri: appBaseUrl,
  })

  const logoutUrl = `${keycloakUrl}/realms/${config.public.keycloakRealm}/protocol/openid-connect/logout?${params}`
  return sendRedirect(event, logoutUrl)
})
