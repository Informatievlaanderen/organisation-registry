/**
 * POST /api/allowed
 * Server-side call: haal organisatie op en update de naam.
 *
 * Gebruikt het via RFC 8693 token exchange verkregen token als Bearer.
 * Dit token is uitgegeven door Keycloak met audience=organisation-registry-api.
 *
 * De API valideert het token via OAuth2 introspection (BffApi scheme).
 * De introspection response bevat iv_wegwijs_rol_3D claims die via BffClaimsTransformation
 * worden omgezet naar interne ClaimTypes.Role claims.
 *
 * Verwacht resultaat: 200 als de gebruiker beheerder is van de geselecteerde organisatie,
 * 403 als die rol ontbreekt.
 */
import { defineEventHandler, readBody } from 'h3'
import { getSession } from '../utils/session'

export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()
  const session = getSession(event, config.sessionSecret)

  if (!session.accessToken) {
    return { error: 'Niet ingelogd', status: 401 }
  }

  const body = await readBody(event)
  const organisationId = body?.organisationId
  const newName = body?.name

  if (!organisationId || !newName) {
    return { error: 'organisationId en name zijn verplicht', status: 400 }
  }

  // Bepaal welk token we gebruiken:
  // - exchangedToken: via RFC 8693 exchange verkregen, audience=organisation-registry-api
  // - fallback naar accessToken als exchange mislukt was (voor demo: toont dan 401)
  const bearerToken = session.exchangedToken ?? session.accessToken
  const tokenSource = session.exchangedToken ? 'exchanged (RFC 8693)' : 'direct Keycloak access token (exchange mislukt)'

  console.log('bearerToken', bearerToken)

  // Haal eerst de huidige organisatiegegevens op (GET is niet beveiligd)
  let currentOrg: any
  try {
    currentOrg = await $fetch(`${config.apiBaseUrl}/organisations/${organisationId}`, {
      headers: { Accept: 'application/json' },
    })
  } catch (err: any) {
    return {
      status: err?.response?.status ?? 502,
      statusText: 'Ophalen organisatie mislukt',
      error: err?.message,
    }
  }

  // PUT met het exchanged token — [OrganisationRegistryAuthorize] verwacht custom JWT scheme
  const putUrl = `${config.apiBaseUrl}/organisations/${organisationId}`
  let status = 0
  let statusText = ''
  let responseBody: any = null

  console.log('putUrl', putUrl)

  try {
    await $fetch(putUrl, {
      method: 'PUT',
      headers: {
        Authorization: `Bearer ${bearerToken}`,
        'Content-Type': 'application/json',
        Accept: 'application/json',
      },
      body: JSON.stringify({
        name: newName,
        description: currentOrg.description ?? '',
        shortName: currentOrg.shortName ?? '',
        purposeIds: currentOrg.purposeIds ?? [],
        showOnVlaamseOverheidSites: currentOrg.showOnVlaamseOverheidSites ?? false,
        validFrom: currentOrg.validFrom ?? null,
        validTo: currentOrg.validTo ?? null,
        article: currentOrg.article ?? null,
        operationalValidFrom: currentOrg.operationalValidFrom ?? null,
        operationalValidTo: currentOrg.operationalValidTo ?? null,
      }),
    })
    status = 200
    statusText = 'OK'
    responseBody = { success: true }
  } catch (err: any) {
    console.log('error', JSON.stringify(err))
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
    tokenExchangeUsed: !!session.exchangedToken,
    tokenSource,
    request: {
      method: 'PUT',
      url: putUrl,
      authScheme: `Bearer (${tokenSource}, server-side)`,
    },
  }
})
