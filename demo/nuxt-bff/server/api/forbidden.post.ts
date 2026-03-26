/**
 * POST /api/forbidden
 * Server-side call naar een endpoint zonder auth (GET /v1/organisations) —
 * daarna een call naar een Admin endpoint dat AlgemeenBeheerder vereist.
 * cjmClient scope heeft geen AlgemeenBeheerder rol → 403/401.
 *
 * Dit toont dat ook na een correcte login, scope-gebaseerde toegang
 * anders werkt dan rol-gebaseerde toegang.
 */
import { defineEventHandler } from 'h3'
import { getSession } from '../utils/session'

export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()
  const session = getSession(event, config.sessionSecret)

  if (!session.accessToken) {
    return { error: 'Niet ingelogd', status: 401 }
  }

  // Roep een admin endpoint aan dat [OrganisationRegistryAuthorize(Role.AlgemeenBeheerder)] heeft
  // Dit endpoint vereist het custom JWT scheme + AlgemeenBeheerder rol — beide ontbreken
  const forbiddenUrl = `${config.apiBaseUrl}/events`

  let status = 0
  let statusText = ''
  let responseBody: any = null

  try {
    await $fetch(forbiddenUrl, {
      method: 'GET',
      headers: {
        Authorization: `Bearer ${session.accessToken}`,
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
      authScheme: 'Bearer (Keycloak access token, server-side)',
    },
  }
})
