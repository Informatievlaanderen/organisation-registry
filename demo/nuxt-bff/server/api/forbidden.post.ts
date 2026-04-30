/**
 * POST /api/forbidden
 * Server-side call naar een Admin endpoint dat AlgemeenBeheerder of Developer vereist.
 * Gebruikt hetzelfde exchanged token als /api/me en /api/allowed.
 */
import { defineEventHandler } from 'h3'
import { getSession } from '../utils/session'

export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()
  const session = getSession(event, config.sessionSecret)

  if (!session.accessToken) {
    return { error: 'Niet ingelogd', status: 401 }
  }

  // Roep een admin endpoint aan dat [OrganisationRegistryAuthorize(Role.AlgemeenBeheerder, Role.Developer)] heeft.
  const forbiddenUrl = `${config.apiBaseUrl}/events`
  const bearerToken = session.exchangedToken ?? session.accessToken
  const tokenSource = session.exchangedToken ? 'exchanged (RFC 8693)' : 'direct Keycloak access token (exchange mislukt)'

  let status = 0
  let statusText = ''
  let responseBody: any = null

  try {
    await $fetch(forbiddenUrl, {
      method: 'GET',
      headers: {
        Authorization: `Bearer ${bearerToken}`,
        Accept: 'application/json',
      },
    })
    status = 200
    statusText = 'OK'
    responseBody = { success: true }
  } catch (err: any) {
    status = err?.response?.status ?? 502
    statusText = err?.response?.statusText ?? err?.message ?? 'Fout'
    try {
      responseBody = await err?.response?.json()
    } catch {
      responseBody = err?.message
    }
  }

  return {
    status,
    statusText,
    body: responseBody,
    request: {
      method: 'GET',
      url: forbiddenUrl,
      authScheme: `Bearer (${tokenSource}, server-side)`,
    },
  }
})
