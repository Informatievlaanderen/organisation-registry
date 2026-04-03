/**
 * URL utilities — derive app and Keycloak URLs dynamically from the incoming Host header.
 *
 * This makes the Nuxt BFF work on any IP/hostname without redeployment:
 *   app.localhost:9080        → appBaseUrl = http://app.localhost:9080
 *                               keycloakUrl = http://keycloak.localhost:9080
 *   app.10.20.20.100.nip.io:9080 → appBaseUrl = http://app.10.20.20.100.nip.io:9080
 *                                   keycloakUrl = http://keycloak.10.20.20.100.nip.io:9080
 *
 * Falls back to NUXT_PUBLIC_APP_BASE_URL / NUXT_PUBLIC_KEYCLOAK_URL env vars when the
 * Host header is absent (e.g. during SSR build or direct internal calls).
 */
import { H3Event, getRequestHeader } from 'h3'

/**
 * Returns the base URL of the current request, e.g. "http://app.10.20.20.100.nip.io:9080".
 * The trailing slash is NOT included.
 */
export function getAppBaseUrl(event: H3Event): string {
  const host = getRequestHeader(event, 'host')
  if (host) {
    return `http://${host}`
  }
  // Fallback to static config (local dev without proxy, or build time)
  const config = useRuntimeConfig()
  return config.public.appBaseUrl
}

/**
 * Returns the Keycloak URL derived from the current request host by replacing the
 * leading subdomain (e.g. "app") with "keycloak".
 *
 * e.g. "http://app.10.20.20.100.nip.io:9080" → "http://keycloak.10.20.20.100.nip.io:9080"
 */
export function getKeycloakUrl(event: H3Event): string {
  const host = getRequestHeader(event, 'host')
  if (host) {
    // Replace leading subdomain with "keycloak"
    const keycloakHost = host.replace(/^[^.]+\./, 'keycloak.')
    return `http://${keycloakHost}`
  }
  const config = useRuntimeConfig()
  return config.public.keycloakUrl
}
