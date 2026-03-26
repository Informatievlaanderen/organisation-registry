/**
 * POST /api/allowed
 * Server-side call: haal organisatie op en update de naam.
 * Gebruikt het Keycloak access token als Bearer → stuurt naar
 * PUT /v1/organisations/{id} die [OrganisationRegistryAuthorize] heeft.
 *
 * Dit zal 401 teruggeven omdat OrganisationRegistryAuthorize het JwtBearer
 * scheme verwacht (custom JWT), niet het Keycloak access token.
 *
 * Dit is de "allowed" call in de demo: de gebruiker heeft de juiste rechten
 * (cjmbeheerder scope) maar het token-formaat wordt niet geaccepteerd.
 * Zodra de auth op policy i.p.v. scheme staat, zal dit wél werken.
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

  // PUT met het Keycloak access token — OrganisationRegistryAuthorize verwacht custom JWT
  const putUrl = `${config.apiBaseUrl}/organisations/${organisationId}`
  let status = 0
  let statusText = ''
  let responseBody: any = null

  try {
    await $fetch(putUrl, {
      method: 'PUT',
      headers: {
        Authorization: `Bearer ${session.accessToken}`,
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
    note: status === 401
      ? 'De API weigert het Keycloak access token — OrganisationRegistryAuthorize verwacht het custom JWT scheme. Dit is het verwachte gedrag totdat de autorisatie op policy wordt gezet.'
      : null,
    request: {
      method: 'PUT',
      url: putUrl,
      // Access token NOOIT meesturen naar browser — dit is de BFF verantwoordelijkheid
      authScheme: 'Bearer (Keycloak access token, server-side)',
    },
  }
})
