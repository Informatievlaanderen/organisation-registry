/**
 * GET /api/me
 * Geeft de ingelogde gebruiker terug (uit de sessie), zonder tokens te lekken.
 * Bevat ook de status van de token exchange zodat de UI dit kan tonen.
 */
import { defineEventHandler } from 'h3'
import { getSession } from '../utils/session'

export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()
  const session = getSession(event, config.sessionSecret)

  if (!session.accessToken) {
    return { loggedIn: false }
  }

  // Decodeer het Keycloak JWT payload (geen verificatie — we vertrouwen onze eigen sessie)
  try {
    const parts = session.accessToken.split('.')
    const payload = JSON.parse(Buffer.from(parts[1], 'base64').toString('utf8'))
    return {
      loggedIn: true,
      sub: payload.sub,
      clientId: payload.azp || payload.client_id,
      scope: payload.scope,
      exp: payload.exp,
      // Token exchange status
      tokenExchange: session.exchangedToken
        ? { success: true }
        : { success: false, error: session.exchangeError ?? 'onbekende fout' },
    }
  } catch {
    return { loggedIn: false }
  }
})
