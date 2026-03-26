// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  ssr: true,

  runtimeConfig: {
    // Server-only (nooit naar browser)
    keycloakClientSecret: process.env.NUXT_BFF_CLIENT_SECRET || '',
    sessionSecret: process.env.NUXT_SESSION_SECRET || 'change-me-in-production-32chars!!',

    public: {
      // Beschikbaar in browser én server
      keycloakUrl: process.env.NUXT_PUBLIC_KEYCLOAK_URL || 'http://localhost:8180',
      keycloakRealm: process.env.NUXT_PUBLIC_KEYCLOAK_REALM || 'wegwijs',
      keycloakClientId: process.env.NUXT_PUBLIC_KEYCLOAK_CLIENT_ID || 'nuxt-bff',
      appBaseUrl: process.env.NUXT_PUBLIC_APP_BASE_URL || 'http://localhost:5090',
    },

    // Server-only API URL (intern Docker netwerk)
    apiBaseUrl: process.env.NUXT_API_BASE_URL || 'http://localhost:9002/v1',
    // Keycloak intern (voor server-side token requests)
    keycloakInternalUrl: process.env.NUXT_KEYCLOAK_INTERNAL_URL || 'http://localhost:8180',
  },

  nitro: {
    preset: 'node-server',
  },
})
