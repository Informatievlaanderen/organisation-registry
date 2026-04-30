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

  // Decodeer het Keycloak JWT payload (geen verificatie; de sessiecookie is server-side beheerd)
  try {
    const parts = session.accessToken.split('.')
    const payload = JSON.parse(Buffer.from(parts[1], 'base64').toString('utf8'))
    const security = await getApiSecurityInformation(config.apiBaseUrl, session.exchangedToken ?? session.accessToken, !!session.exchangedToken)

    return {
      loggedIn: true,
      sub: payload.sub,
      clientId: payload.azp || payload.client_id,
      scope: payload.scope,
      exp: payload.exp,
      security,
      // Token exchange status
      tokenExchange: session.exchangedToken
        ? { success: true }
        : { success: false, error: session.exchangeError ?? 'onbekende fout' },
    }
  } catch {
    return { loggedIn: false }
  }
})

async function getApiSecurityInformation(apiBaseUrl: string, bearerToken: string, tokenExchangeUsed: boolean) {
  const url = `${apiBaseUrl}/security`
  const tokenSource = tokenExchangeUsed
    ? 'exchanged (RFC 8693)'
    : 'direct Keycloak access token (exchange mislukt)'

  try {
    const body = await $fetch(url, {
      headers: {
        Authorization: `Bearer ${bearerToken}`,
        Accept: 'application/json',
      },
    })

    return {
      success: true,
      status: 200,
      statusText: 'OK',
      body,
      request: {
        method: 'GET',
        url,
        authScheme: `Bearer (${tokenSource}, server-side)`,
      },
    }
  } catch (err: any) {
    let body: any = err?.message
    try {
      body = await err?.response?.json()
    } catch {
      // Houd de oorspronkelijke foutboodschap.
    }

    return {
      success: false,
      status: err?.response?.status ?? 502,
      statusText: err?.response?.statusText ?? err?.message ?? 'Fout',
      body,
      request: {
        method: 'GET',
        url,
        authScheme: `Bearer (${tokenSource}, server-side)`,
      },
    }
  }
}
