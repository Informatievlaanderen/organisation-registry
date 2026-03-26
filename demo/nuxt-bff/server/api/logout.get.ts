/**
 * GET /api/logout
 * Wist de sessie en redirect naar Keycloak logout.
 */
import { defineEventHandler, sendRedirect } from 'h3'
import { clearSession } from '../utils/session'

export default defineEventHandler((event) => {
  const config = useRuntimeConfig()
  clearSession(event)
  const logoutUrl = `${config.public.keycloakUrl}/realms/${config.public.keycloakRealm}/protocol/openid-connect/logout?client_id=${config.public.keycloakClientId}&post_logout_redirect_uri=${encodeURIComponent(config.public.appBaseUrl)}`
  return sendRedirect(event, logoutUrl)
})
