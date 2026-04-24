<template>
  <div class="container">
    <h1>Nuxt BFF Demo — Organisation Registry</h1>

    <div v-if="!me.loggedIn" class="card">
      <p>
        Demonstreert de Authorization Code + PKCE flow via Keycloak.<br />
        De BFF beheert het token server-side — het token is nooit zichtbaar in de browser.
      </p>
      <a href="/api/login" class="btn btn-primary">Inloggen via Keycloak</a>
      <div v-if="error" class="alert alert-error">{{ error }}</div>
    </div>

    <template v-else>
      <div class="card status-bar">
        <span>Ingelogd als <strong>{{ me.clientId }}</strong></span>
        <span>Scope: <code>{{ me.scope }}</code></span>
        <a href="/api/logout" class="btn btn-sm btn-secondary">Uitloggen</a>
      </div>

      <!-- Token exchange status -->
      <div class="card" :class="me.tokenExchange?.success ? 'card-ok' : 'card-warn'">
        <h2>
          <span v-if="me.tokenExchange?.success" class="badge badge-ok">Token exchange</span>
          <span v-else class="badge badge-warning">Token exchange</span>
          RFC 8693 (Keycloak standard V2)
        </h2>
        <p class="explanation" v-if="me.tokenExchange?.success">
          De BFF heeft het Keycloak access token (audience: <code>nuxt-bff</code>) succesvol
          ingewisseld voor een nieuw token met audience: <code>organisation-registry-api</code>.<br />
          Dit token wordt nu gebruikt voor API-calls. Het token zelf is nooit zichtbaar in de browser.
        </p>
        <p class="explanation" v-else>
          Token exchange mislukt: <strong>{{ me.tokenExchange?.error }}</strong><br />
          API-calls gebruiken het originele Keycloak access token (fallback).
        </p>
      </div>

      <!-- Organisatie kiezen -->
      <div class="card">
        <h2>Organisatie</h2>
        <div class="form-row">
          <label>OVO-nummer</label>
          <input v-model="ovoNumber" placeholder="OVO000001" @keyup.enter="loadOrg" />
          <button class="btn btn-secondary" @click="loadOrg" :disabled="loading">Laden</button>
        </div>
        <div v-if="org" class="org-detail">
          <table>
            <tr><th>Id</th><td><code>{{ org.id }}</code></td></tr>
            <tr><th>OVO</th><td>{{ org.ovoNumber }}</td></tr>
            <tr><th>Naam</th><td>{{ org.name }}</td></tr>
            <tr><th>Beschrijving</th><td>{{ org.description || '—' }}</td></tr>
          </table>
        </div>
        <div v-if="orgError" class="alert alert-error">{{ orgError }}</div>
      </div>

      <!-- Allowed: update naam -->
      <div class="card">
        <h2>
          <span class="badge badge-ok">Allowed</span>
          PUT /v1/organisations/{id}
        </h2>
        <p class="explanation">
          De BFF stuurt het <strong>via RFC 8693 exchanged token</strong> (audience:
          <code>organisation-registry-api</code>) als Bearer naar
          <code>PUT /v1/organisations/{id}</code>.<br />
          De API valideert via <strong>OAuth2 introspection</strong> (BffApi scheme) en zet
          <code>iv_wegwijs_rol_3D</code> claims om naar interne rollen.<br />
          Resultaat: <strong>200</strong> als de gebruiker beheerder is van de geselecteerde organisatie,
          <strong>403</strong> als die rol ontbreekt.
        </p>
        <div v-if="org" class="form-row">
          <label>Nieuwe naam</label>
          <input v-model="newName" :placeholder="org.name" />
          <button class="btn btn-success" @click="doAllowed" :disabled="!org || loading">
            Uitvoeren
          </button>
        </div>
        <div v-else class="muted">Laad eerst een organisatie hierboven.</div>
        <div v-if="allowedResult" class="result" :class="resultClass(allowedResult.status)">
          <div class="result-status">HTTP {{ allowedResult.status }} {{ allowedResult.statusText }}</div>
          <div v-if="allowedResult.note" class="result-note">{{ allowedResult.note }}</div>
          <pre>{{ JSON.stringify(allowedResult.body, null, 2) }}</pre>
          <div class="result-request">
            Request: <code>{{ allowedResult.request?.method }} {{ allowedResult.request?.url }}</code>
            ({{ allowedResult.request?.authScheme }})
          </div>
        </div>
      </div>

      <!-- Forbidden: admin endpoint -->
      <div class="card">
        <h2>
          <span class="badge badge-danger">Forbidden</span>
          GET /v1/events
        </h2>
        <p class="explanation">
          De BFF roept <code>GET /v1/events</code> aan met het Keycloak access token.<br />
          Dit endpoint heeft <code>[OrganisationRegistryAuthorize(Role.AlgemeenBeheerder)]</code>.<br />
          Resultaat: <strong>401</strong> — verkeerd scheme én verkeerde rol.
        </p>
        <button class="btn btn-danger" @click="doForbidden" :disabled="loading">Uitvoeren</button>
        <div v-if="forbiddenResult" class="result" :class="resultClass(forbiddenResult.status)">
          <div class="result-status">HTTP {{ forbiddenResult.status }} {{ forbiddenResult.statusText }}</div>
          <pre>{{ JSON.stringify(forbiddenResult.body, null, 2) }}</pre>
          <div class="result-request">
            Request: <code>{{ forbiddenResult.request?.method }} {{ forbiddenResult.request?.url }}</code>
            ({{ forbiddenResult.request?.authScheme }})
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup>
const route = useRoute()
const error = ref(route.query.error || null)
const loading = ref(false)
const ovoNumber = ref('OVO000001')
const newName = ref('')
const org = ref(null)
const orgError = ref(null)
const allowedResult = ref(null)
const forbiddenResult = ref(null)

const { data: me } = await useFetch('/api/me', { default: () => ({ loggedIn: false }) })

async function loadOrg() {
  orgError.value = null
  org.value = null
  loading.value = true
  try {
    org.value = await $fetch(`/api/organisation?ovo=${ovoNumber.value}`)
    newName.value = org.value.name
  } catch (err) {
    orgError.value = err?.message || 'Organisatie niet gevonden'
  } finally {
    loading.value = false
  }
}

async function doAllowed() {
  if (!org.value) return
  loading.value = true
  allowedResult.value = null
  try {
    allowedResult.value = await $fetch('/api/allowed', {
      method: 'POST',
      body: { organisationId: org.value.id, name: newName.value || org.value.name },
    })
  } catch (err) {
    allowedResult.value = { status: 0, statusText: err?.message }
  } finally {
    loading.value = false
  }
}

async function doForbidden() {
  loading.value = true
  forbiddenResult.value = null
  try {
    forbiddenResult.value = await $fetch('/api/forbidden', { method: 'POST' })
  } catch (err) {
    forbiddenResult.value = { status: 0, statusText: err?.message }
  } finally {
    loading.value = false
  }
}

function resultClass(status) {
  if (status >= 200 && status < 300) return 'result-ok'
  if (status === 401 || status === 403) return 'result-forbidden'
  return 'result-error'
}
</script>

<style scoped>
.container { font-family: sans-serif; max-width: 900px; margin: 40px auto; padding: 0 20px; }
h1 { font-size: 1.4rem; margin-bottom: 24px; }
h2 { font-size: 1.1rem; margin: 0 0 12px; display: flex; align-items: center; gap: 8px; }
.card { background: #fff; border: 1px solid #ddd; border-radius: 6px; padding: 20px; margin-bottom: 20px; }
.status-bar { display: flex; align-items: center; gap: 16px; background: #f0f8e8; border-color: #b7e0a0; }
.card-ok { background: #d4edda; border-color: #c3e6cb; }
.card-warn { background: #fff3cd; border-color: #ffc107; }
.badge-ok { background: #d4edda; color: #155724; border: 1px solid #c3e6cb; }
.form-row { display: flex; gap: 8px; align-items: center; margin-top: 12px; }
.form-row label { min-width: 100px; font-size: 0.9rem; }
.form-row input { flex: 1; padding: 6px 10px; border: 1px solid #ccc; border-radius: 4px; font-size: 0.95rem; }
.btn { padding: 8px 16px; border: none; border-radius: 4px; cursor: pointer; font-size: 0.95rem; text-decoration: none; display: inline-block; }
.btn:disabled { opacity: 0.5; cursor: not-allowed; }
.btn-primary { background: #0066cc; color: #fff; }
.btn-secondary { background: #6c757d; color: #fff; }
.btn-success { background: #28a745; color: #fff; }
.btn-danger { background: #dc3545; color: #fff; }
.btn-sm { padding: 4px 10px; font-size: 0.85rem; }
.badge { font-size: 0.75rem; padding: 2px 8px; border-radius: 12px; font-weight: bold; }
.badge-warning { background: #fff3cd; color: #856404; border: 1px solid #ffc107; }
.badge-danger { background: #f8d7da; color: #721c24; border: 1px solid #dc3545; }
.explanation { color: #555; font-size: 0.9rem; line-height: 1.5; margin: 8px 0 16px; }
.muted { color: #888; font-size: 0.9rem; }
.org-detail { margin-top: 12px; }
.org-detail table { width: 100%; border-collapse: collapse; font-size: 0.9rem; }
.org-detail th { text-align: left; padding: 4px 12px 4px 0; color: #555; width: 120px; }
.org-detail td { padding: 4px 0; }
.result { margin-top: 16px; border-radius: 4px; padding: 12px 16px; font-size: 0.85rem; }
.result-ok { background: #d4edda; border: 1px solid #c3e6cb; }
.result-forbidden { background: #fff3cd; border: 1px solid #ffc107; }
.result-error { background: #f8d7da; border: 1px solid #f5c6cb; }
.result-status { font-weight: bold; margin-bottom: 4px; }
.result-note { font-style: italic; margin-bottom: 8px; color: #555; }
.result-request { margin-top: 8px; color: #555; }
pre { background: #f4f4f4; padding: 8px; border-radius: 4px; white-space: pre-wrap; word-break: break-all; margin: 8px 0 0; }
.alert { padding: 10px 14px; border-radius: 4px; margin-top: 12px; }
.alert-error { background: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }
code { background: #f4f4f4; padding: 1px 5px; border-radius: 3px; font-size: 0.9em; }
</style>
