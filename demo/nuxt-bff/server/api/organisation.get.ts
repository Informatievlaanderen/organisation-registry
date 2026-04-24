/**
 * GET /api/organisation?ovo=OVO000099
 * Proxy naar de Organisation Registry API (geen auth vereist voor GET).
 */
import { defineEventHandler, getQuery } from 'h3'

export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()
  const query = getQuery(event)
  const ovo = query.ovo as string

  if (!ovo) {
    throw createError({ statusCode: 400, statusMessage: 'ovo parameter is verplicht' })
  }

  const url = `${config.apiBaseUrl}/organisations/${ovo}`
  try {
    return await $fetch(url, { headers: { Accept: 'application/json' } })
  } catch (err: any) {
    throw createError({
      statusCode: err?.response?.status ?? 502,
      statusMessage: `Organisatie niet gevonden: ${ovo}`,
    })
  }
})
