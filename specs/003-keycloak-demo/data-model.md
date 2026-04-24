# Data Model: Keycloak Demo — All New Files

**Branch**: `003-keycloak-demo` | **Date**: 2026-03-23  
**Purpose**: Complete inventory of every file to be created or modified for this feature.

---

## Legend

| Symbol | Meaning |
|---|---|
| NEW | File does not exist; must be created |
| MOD | File exists; must be modified |

---

## US1 — Keycloak Realm + API Config

### `keycloak/realm-export.json` — NEW

**Purpose**: Keycloak realm import file. Auto-imported in de actieve Tilt/k3d setup via de
`keycloak-realm-configmap` local resource en de Keycloak deployment.

**Contains**:
- Realm `wegwijs` with `enabled: true`
- Client scopes: `dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_orafinbeheerder`,
  `dv_organisatieregister_info`, `dv_organisatieregister_testclient`
- Client `organisation-registry-local-dev` (Authorization Code + PKCE):
  - `secret: a_very=Secr3t*Key`
  - `validRedirectUris`: the two Angular SPA URIs + wildcard
  - `webOrigins`: `https://organisatie.dev-vlaanderen.local`
  - Protocol mappers: `iv_wegwijs_rol_3D` (multivalued User Attribute), `vo_id` (User Attribute)
  - Allowed scopes: openid, profile, `dv_organisatieregister_cjmbeheerder`
- Client `cjmClient` (Client Credentials):
  - `secret: secret`
  - `serviceAccountsEnabled: true`
  - Allowed scopes: `dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_info`
- Client `organisation-registry-api` (resource server for introspection):
  - `secret: a_very=Secr3t*Key`
  - `bearerOnly: false`, `standardFlowEnabled: false`, `directAccessGrantsEnabled: false`
- Client `nuxt-bff` (Authorization Code + PKCE):
  - `secret: nuxt-bff-secret`
  - `validRedirectUris`: `http://app.localhost:9080/callback`
  - `webOrigins`: `http://app.localhost:9080`
  - Protocol mappers: `iv_wegwijs_rol_3D`, `vo_id` (same as SPA client)
  - Allowed scopes: openid, profile, `dv_organisatieregister_cjmbeheerder`
- Users (7 test users mirroring Duende `Config.cs`):
  - `dev` / `dev` — algemeenbeheerder + other roles
  - `vlimpers` / `vlimpers`
  - `decentraalbeheerder` / `decentraalbeheerder`
  - `algemeenbeheerder` / `algemeenbeheerder`
  - `organen` / `organen`
  - `regelgeving` / `regelgeving`
  - `cjmbeheerder` / `cjmbeheerder`
  Each with `attributes.vo_id` and `attributes.iv_wegwijs_rol_3D` (array)

---

### `src/OrganisationRegistry.Api/appsettings.keycloak.json` — NEW

**Purpose**: ASP.NET Core configuration override applied when `ASPNETCORE_ENVIRONMENT=Keycloak`.
Replaces all Duende IdentityServer endpoints with Keycloak 24.0 equivalents for the `wegwijs` realm.

**Overrides these sections** (relative to `appsettings.development.json`):

| Section | Key | Keycloak value |
|---|---|---|
| `OIDCAuth` | `Authority` | `http://keycloak:8080/realms/wegwijs` |
| `OIDCAuth` | `TokenEndPoint` | `/protocol/openid-connect/token` |
| `OIDCAuth` | `AuthorizationEndpoint` | `http://keycloak.localhost:9080/realms/wegwijs/protocol/openid-connect/auth` |
| `OIDCAuth` | `UserInfoEndPoint` | `http://keycloak.localhost:9080/realms/wegwijs/protocol/openid-connect/userinfo` |
| `OIDCAuth` | `EndSessionEndPoint` | `http://keycloak.localhost:9080/realms/wegwijs/protocol/openid-connect/logout` |
| `OIDCAuth` | `AuthorizationIssuer` | `http://keycloak.localhost:9080/realms/wegwijs` |
| `OIDCAuth` | `JwksUri` | `http://keycloak:8080/realms/wegwijs/protocol/openid-connect/certs` |
| `OIDCAuth` | `ClientId` | `organisation-registry-local-dev` |
| `OIDCAuth` | `ClientSecret` | `a_very=Secr3t*Key` |
| `OIDCAuth` | `AuthorizationRedirectUri` | `https://organisatie.dev-vlaanderen.local/#/oic` |
| `OIDCAuth` | `JwtSharedSigningKey` | `a_shared_signing_key` (unchanged) |
| `OIDCAuth` | `JwtIssuer` | `https://organisatie.dev-vlaanderen.local` (unchanged) |
| `OIDCAuth` | `JwtAudience` | `https://organisatie.dev-vlaanderen.local` (unchanged) |
| `EditApi` | `ClientId` | `organisation-registry-api` |
| `EditApi` | `ClientSecret` | `a_very=Secr3t*Key` |
| `EditApi` | `Authority` | `http://keycloak:8080/realms/wegwijs` |
| `EditApi` | `IntrospectionEndpoint` | `http://keycloak:8080/realms/wegwijs/protocol/openid-connect/token/introspect` |
| `FeatureManagement` | `EditApi` | `true` |

---

### `Tiltfile` + `demo/k8s/*.yaml` — MOD

**Changes**:
1. Keycloak deployment/config: externe URL op `keycloak.localhost:9080`, interne URL via service `http://keycloak`
2. `m2m-demo` deployment/service (US2)
3. `nuxt-bff` deployment/service (US3)

---

## US2 — M2M Demo (.NET 8 Minimal API)

### `demo/m2m/m2m.csproj` — NEW

**Purpose**: .NET 8 project file for the M2M demo web UI.  
**Dependencies**: `Microsoft.AspNetCore.App` framework reference only. No extra NuGet packages;
uses `HttpClient` (built-in) for token + API calls and `Microsoft.AspNetCore.Http` for the
minimal API endpoints. Inline HTML is served as a string literal.

### `demo/m2m/Program.cs` — NEW

**Purpose**: Single-file .NET 8 Minimal API. Serves:

- `GET /` — HTML page with three buttons (Authenticate, Allowed call, Forbidden call), a results
  `<pre>` block, and minimal JavaScript (`fetch` calls to the three POST endpoints below)
- `POST /demo/authenticate` — acquires Client Credentials token from Keycloak; stores in a
  static in-memory holder (acceptable for single-user demo); returns JSON `{ expires_in, scope }`
- `POST /demo/allowed` — uses stored token to `POST /edit/organisations/{TEST_OVO_ID}/contacts`
  on the API; returns JSON `{ status, body }`
- `POST /demo/forbidden` — uses stored token to `GET /edit/organisations/{TEST_OVO_ID}/keys`
  on the API; returns JSON `{ status, body }`

**Configuration** (read from environment variables):
- `KEYCLOAK_TOKEN_ENDPOINT`
- `CJM_CLIENT_ID`
- `CJM_CLIENT_SECRET`
- `API_BASE_URL`
- `TEST_OVO_ID`

**Important**: The stored access token is a demo-only in-memory singleton. Not thread-safe for
production but sufficient for a single-developer demo.

### `demo/m2m/Dockerfile` — NEW

**Purpose**: Multi-stage Dockerfile.
- Stage 1 (`build`): `mcr.microsoft.com/dotnet/sdk:8.0` — `dotnet publish -c Release -o /app/publish`
- Stage 2 (`runtime`): `mcr.microsoft.com/dotnet/aspnet:8.0` — copies published output, `EXPOSE 8080`

---

## US3 — Nuxt 3 BFF

### `demo/nuxt-bff/package.json` — NEW

**Purpose**: Node.js package manifest.  
**Dependencies**:
- `nuxt: ^3.x`
- `ofetch` (included in Nuxt via `$fetch`)
- No extra auth libraries (custom `h3` handlers)

**DevDependencies**: TypeScript (included via Nuxt)

### `demo/nuxt-bff/nuxt.config.ts` — NEW

**Purpose**: Nuxt 3 config. Sets:
- `ssr: true` (server-side rendering required for security model)
- `runtimeConfig` for environment variables (KEYCLOAK_AUTH_URL, BFF_CLIENT_ID, etc.)
- `nitro.preset: 'node-server'` for Docker deployment

### `demo/nuxt-bff/server/routes/login.get.ts` — NEW

**Purpose**: Generates PKCE `code_verifier` + `code_challenge`, stores `code_verifier` in session,
redirects user to Keycloak authorization endpoint with `response_type=code`, `scope=openid profile
dv_organisatieregister_cjmbeheerder`, `code_challenge`, `redirect_uri`.

### `demo/nuxt-bff/server/routes/callback.get.ts` — NEW

**Purpose**: Receives authorization code from Keycloak. Retrieves `code_verifier` from session.
Exchanges code for tokens at `KEYCLOAK_TOKEN_URL` (internal Kubernetes service URL). Stores `access_token`
in the encrypted session (using Nitro's `useSession`). Redirects to `/dashboard`.  
The `access_token` is **never** written to a response body.

### `demo/nuxt-bff/server/routes/logout.get.ts` — NEW

**Purpose**: Clears the server session and redirects to Keycloak logout endpoint.

### `demo/nuxt-bff/server/api/allowed.get.ts` — NEW

**Purpose**: Server-side API route. Reads `access_token` from session. Makes
`GET /edit/organisations/{TEST_OVO_ID}/contacts` request to the Organisation Registry API with
`Authorization: Bearer {access_token}`. Returns `{ status, data }` JSON.  
Called from `dashboard.vue` via `useFetch('/api/allowed')`.

### `demo/nuxt-bff/server/api/forbidden.get.ts` — NEW

**Purpose**: Server-side API route. Reads `access_token` from session. Makes
`GET /edit/organisations/{TEST_OVO_ID}/keys` request to the Organisation Registry API
(policy `Keys`, requires `dv_organisatieregister_orafinbeheerder`). Returns `{ status }` JSON.
Expected result: `403`.  
Called from `dashboard.vue` via `useFetch('/api/forbidden')`.

### `demo/nuxt-bff/server/middleware/auth.ts` — NEW

**Purpose**: Nitro server middleware that runs on every non-public route. Checks if
`access_token` exists in session. If not, redirects to `/login`.  
Bypasses itself for `/login`, `/callback`, and `/api/*` routes to avoid redirect loops.

### `demo/nuxt-bff/pages/index.vue` — NEW

**Purpose**: Landing page (unauthenticated). Shows a "Log in via Keycloak" button that links to
`/login`. If user is already authenticated (session check), redirects to `/dashboard`.

### `demo/nuxt-bff/pages/dashboard.vue` — NEW

**Purpose**: Protected page. Rendered server-side. Displays:
- User greeting (from session if `id_token` is decoded)
- Allowed call result (`useFetch('/api/allowed')`) — shows status + data excerpt
- Forbidden call result (`useFetch('/api/forbidden')`) — shows `403 Forbidden` as expected
- Logout link (`/logout`)

### `demo/nuxt-bff/Dockerfile` — NEW

**Purpose**: Multi-stage Dockerfile.
- Stage 1 (`build`): `node:20-alpine` — `npm ci && npm run build`
- Stage 2 (`runtime`): `node:20-alpine` — copies `.output/`, `EXPOSE 3000`, `CMD ["node", ".output/server/index.mjs"]`

---

## Summary: New Files Count

| Path | Type | US |
|---|---|---|
| `keycloak/realm-export.json` | JSON | US1 |
| `src/OrganisationRegistry.Api/appsettings.keycloak.json` | JSON | US1 |
| `Tiltfile` + `demo/k8s/*` | MOD | US1/2/3 |
| `demo/m2m/m2m.csproj` | XML | US2 |
| `demo/m2m/Program.cs` | C# | US2 |
| `demo/m2m/Dockerfile` | Docker | US2 |
| `demo/nuxt-bff/package.json` | JSON | US3 |
| `demo/nuxt-bff/nuxt.config.ts` | TypeScript | US3 |
| `demo/nuxt-bff/server/routes/login.get.ts` | TypeScript | US3 |
| `demo/nuxt-bff/server/routes/callback.get.ts` | TypeScript | US3 |
| `demo/nuxt-bff/server/routes/logout.get.ts` | TypeScript | US3 |
| `demo/nuxt-bff/server/api/allowed.get.ts` | TypeScript | US3 |
| `demo/nuxt-bff/server/api/forbidden.get.ts` | TypeScript | US3 |
| `demo/nuxt-bff/server/middleware/auth.ts` | TypeScript | US3 |
| `demo/nuxt-bff/pages/index.vue` | Vue | US3 |
| `demo/nuxt-bff/pages/dashboard.vue` | Vue | US3 |
| `demo/nuxt-bff/Dockerfile` | Docker | US3 |

**Total**: 1 modified file + 16 new files
