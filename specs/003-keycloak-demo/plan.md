# Implementation Plan: Keycloak Demo

**Branch**: `003-keycloak-demo` | **Date**: 2026-03-23 | **Spec**: [spec.md](./spec.md)  
**Input**: Feature specification from `/specs/003-keycloak-demo/spec.md`

## Summary

Replace Duende IdentityServer with Keycloak 24.0 as the identity provider for all three authentication
flows used by the Organisation Registry: the Angular SPA (Authorization Code + PKCE with custom JWT
exchange), machine-to-machine (Client Credentials via OAuth2 introspection), and an interactive BFF
pattern (Nuxt 3 server-side token management). Everything runs locally via docker-compose.

The plan is split into three independent but sequentially-dependent user stories:

- **US1** (foundation): Keycloak realm JSON + `appsettings.keycloak.json` so the Angular SPA works
  with Keycloak instead of Duende — without touching Angular source code.
- **US2** (M2M demo): A .NET 8 Minimal API web UI (Docker) with "Authenticate", "Allowed call",
  "Forbidden call" buttons demonstrating Client Credentials against the Edit API.
- **US3** (BFF demo): A Nuxt 3 BFF with server-side authorization code flow, token exchange, and
  allowed/forbidden Edit API calls — access token never reaches the browser.

## Technical Context

**Language/Version**: C# / .NET 8 (M2M demo), JavaScript/TypeScript / Node 20 (Nuxt 3 BFF),
JSON (Keycloak realm config)  
**Primary Dependencies**: Keycloak 24.0, IdentityModel (for `RequestAuthorizationCodeTokenAsync`),
`@sidebase/nuxt-auth` or custom `h3` server middleware (Nuxt 3)  
**Storage**: No new storage. Session state in Nuxt BFF uses encrypted server-side cookie store.  
**Testing**: Manual acceptance scenarios (docker-compose based); no automated tests for demo containers  
**Target Platform**: Docker (all three components), linux/amd64  
**Project Type**: Infrastructure / demo services  
**Performance Goals**: Demo-level; no throughput requirements  
**Constraints**: No changes to Angular SPA source. No changes to existing domain code or events.
`appsettings.keycloak.json` is a pure config override. The API access token must never reach the browser.  
**Scale/Scope**: Local dev environment only

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|---|---|---|
| I. Event Sourcing is Immutable Law | PASS | No events touched |
| II. Commands and Events Drive All State Changes | PASS | No domain state changes |
| III. CQRS: Commands and Queries are Architecturally Separate | PASS | Not applicable to this feature |
| IV. Respect Aggregate Boundaries | PASS | No aggregates modified |
| V. Testing is Not Optional | CONDITIONAL | Demo containers have acceptance scenarios; no unit/integration test required for pure config and demo services per spec priority |

**Complexity justification (new Docker services)**:

| Addition | Why Needed | Simpler Alternative Rejected Because |
|---|---|---|
| `m2m-demo` Docker service | US2 requires browser-accessible M2M demo | Static HTML cannot do server-side Client Credentials safely |
| `nuxt-bff` Docker service | US3 requires server-side token isolation | Browser-side token would violate FR-008 security requirement |
| `keycloak/realm-export.json` | Keycloak `--import-realm` requires a realm JSON file in the import volume | Manual Keycloak UI setup is not reproducible |

## Project Structure

### Documentation (this feature)

```text
specs/003-keycloak-demo/
├── plan.md              # This file
├── research.md          # Phase 0 output (resolved unknowns)
├── data-model.md        # Phase 1 output (all new files)
└── tasks.md             # Phase 2 output (/speckit.tasks — NOT created here)
```

### Source Code Layout

```text
keycloak/
└── realm-export.json                # Keycloak 24.0 realm import (wegwijs realm)

src/OrganisationRegistry.Api/
└── appsettings.keycloak.json        # Config override: OIDCAuth + EditApi → Keycloak endpoints

docker-compose.yml                   # Extended: m2m-demo + nuxt-bff services added

demo/m2m/
├── Dockerfile
├── Program.cs                       # .NET 8 Minimal API — three-button web UI
└── m2m.csproj

demo/nuxt-bff/
├── Dockerfile
├── nuxt.config.ts
├── package.json
├── server/
│   ├── middleware/
│   │   └── auth.ts                  # OIDC session middleware (server-side only)
│   └── routes/
│       ├── callback.ts              # Auth code callback handler
│       └── api/
│           ├── allowed.ts           # Proxies allowed Edit API call
│           └── forbidden.ts         # Proxies forbidden Edit API call
└── pages/
    ├── index.vue                    # Login page
    └── dashboard.vue                # Shows allowed + forbidden results
```

---

## Phase 0 — Research (Resolved Unknowns)

See `research.md` for full analysis. Key conclusions:

1. **Keycloak claim mappers**: Custom attributes on users + "User Attribute" mapper type in the
   realm JSON. Claims `iv_wegwijs_rol_3D` (multi-valued) and `vo_id` can be mapped to the ID token
   and access token by adding mapper definitions inside the `organisation-registry-local-dev` client
   scope or as dedicated client mappers.

2. **Introspection endpoint**: Keycloak 24.0 path is
   `/realms/{realm}/protocol/openid-connect/token/introspect`. For the `wegwijs` realm:
   `http://keycloak:8080/realms/wegwijs/protocol/openid-connect/token/introspect`
   (internal Docker network) configured via `appsettings.keycloak.json`.

3. **Token exchange (Nuxt BFF)**: Keycloak's RFC 8693 token exchange is available but requires
   enabling "Token Exchange" feature flag in Keycloak admin. For the demo, the simpler approach is
   to have the `nuxt-bff` use its own `cjmClient` Client Credentials token directly (no token
   exchange required) or use authorization code flow to get a token scoped to
   `dv_organisatieregister_cjmbeheerder`. The BFF uses authorization code flow; the id_token user
   identity is stored in the session; a separate client-credentials request retrieves an API token.

4. **Docker networking**: Keycloak is dual-addressed:
   - Browser → `http://localhost:8180` (external, for redirects and login UI)
   - Server-side (API, M2M demo, Nuxt BFF) → `http://keycloak:8080` (internal Docker network)
   This means the `Authority` / `IntrospectionEndpoint` in server-side config uses the internal
   hostname, while `AuthorizationEndpoint` / `EndSessionEndPoint` in the `/v1/security/info`
   response (consumed by the Angular SPA browser) must use the external `localhost:8180` address.
   The `appsettings.keycloak.json` sets `Authority` to the internal hostname for token/introspect
   operations, and the public-facing OIDC endpoints to the external hostname.

---

## Phase 1 — Design

### US1: Keycloak Realm + API Config Override

#### Keycloak Realm `wegwijs` (realm-export.json)

The Keycloak realm JSON must configure:

**Realm settings**:
- `realm`: `wegwijs`
- `enabled`: `true`
- `accessTokenLifespan`: 3600 (default; acceptable for demo)
- `registrationAllowed`: `false`

**Clients** (mirrors Duende `Config.cs`):

| clientId | grantType | secret | allowedScopes | redirectUris | webOrigins |
|---|---|---|---|---|---|
| `organisation-registry-local-dev` | `authorization_code` | `a_very=Secr3t*Key` | openid, profile, `iv_wegwijs`, `dv_organisatieregister_cjmbeheerder` | `https://organisatie.dev-vlaanderen.local/#/oic`, `https://organisatie.dev-vlaanderen.local/v2/oic` | `https://organisatie.dev-vlaanderen.local` |
| `cjmClient` | `client_credentials` | `secret` | `dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_info` | — | — |
| `organisation-registry-api` | (resource server for introspection) | `a_very=Secr3t*Key` | — | — | — |
| `nuxt-bff` | `authorization_code` | `nuxt-bff-secret` | openid, profile, `dv_organisatieregister_cjmbeheerder` | `http://localhost:5090/callback` | `http://localhost:5090` |

**Client Scopes** (maps to `ApiScope` in Duende):
- `dv_organisatieregister_cjmbeheerder`
- `dv_organisatieregister_orafinbeheerder`
- `dv_organisatieregister_info`
- `dv_organisatieregister_testclient`

**Protocol Mappers** on `organisation-registry-local-dev` and `nuxt-bff`:
- `iv_wegwijs_rol_3D` — User Attribute mapper, multivalued, add to ID token + access token
- `vo_id` — User Attribute mapper, add to ID token + access token

**Users** (mirrors `Config.cs` `TestUser` list):

| username | password | user attributes |
|---|---|---|
| `dev` | `dev` | `vo_id=9C2F7372-7112-49DC-9771-F127B048B4C7`, `iv_wegwijs_rol_3D=WegwijsBeheerder-algemeenbeheerder:OVO002949` (+ others) |
| `vlimpers` | `vlimpers` | `vo_id=E6D110DC-231A-4666-BAFB-C354255EF547`, `iv_wegwijs_rol_3D=WegwijsBeheerder-vlimpersbeheerder:OVO001833` |
| `decentraalbeheerder` | `decentraalbeheerder` | `vo_id=34E7CF51-0AF1-436E-B187-BEE803525BA6`, `iv_wegwijs_rol_3D=WegwijsBeheerder-decentraalbeheerder:OVO002949` |
| `algemeenbeheerder` | `algemeenbeheerder` | `vo_id=A5C5BFCD-266C-4CC7-9869-4B95E34C090D`, `iv_wegwijs_rol_3D=WegwijsBeheerder-algemeenbeheerder:OVO002949` |
| `cjmbeheerder` | `cjmbeheerder` | `vo_id=A5C5BFCD-266C-4CC7-9869-5B95E34C090E`, `iv_wegwijs_rol_3D=WegwijsBeheerder-cjmbeheerder:OVO002949` |

**`organisation-registry-api` client** is marked `serviceAccountsEnabled: true` and
`bearerOnly: false` so it can be used for introspection by the API. The API authenticates
introspection requests with `clientId=organisation-registry-api` + `clientSecret=a_very=Secr3t*Key`.

#### appsettings.keycloak.json

This file overrides `appsettings.development.json` when `ASPNETCORE_ENVIRONMENT=Keycloak`.
It maps the existing `OIDCAuth` section fields to Keycloak 24.0 endpoints for the `wegwijs` realm.

Key mappings:

| OIDCAuth field | Value | Notes |
|---|---|---|
| `Authority` | `http://keycloak:8080/realms/wegwijs` | Internal Docker, used for token server-side |
| `TokenEndPoint` | `/protocol/openid-connect/token` | Relative to Authority |
| `AuthorizationEndpoint` | `http://localhost:8180/realms/wegwijs/protocol/openid-connect/auth` | External, for browser redirect |
| `UserInfoEndPoint` | `http://localhost:8180/realms/wegwijs/protocol/openid-connect/userinfo` | External |
| `EndSessionEndPoint` | `http://localhost:8180/realms/wegwijs/protocol/openid-connect/logout` | External |
| `AuthorizationIssuer` | `http://localhost:8180/realms/wegwijs` | Must match token `iss` claim for Angular |
| `JwksUri` | `http://keycloak:8080/realms/wegwijs/protocol/openid-connect/certs` | Internal for validation |
| `ClientId` | `organisation-registry-local-dev` | Same as Duende |
| `ClientSecret` | `a_very=Secr3t*Key` | Same as Duende |
| `AuthorizationRedirectUri` | `https://organisatie.dev-vlaanderen.local/#/oic` | Same as Duende |
| `JwtSharedSigningKey` | `a_shared_signing_key` | Unchanged |
| `JwtIssuer` | `https://organisatie.dev-vlaanderen.local` | Unchanged |
| `JwtAudience` | `https://organisatie.dev-vlaanderen.local` | Unchanged |

For `EditApi` introspection:

| EditApi field | Value |
|---|---|
| `ClientId` | `organisation-registry-api` |
| `ClientSecret` | `a_very=Secr3t*Key` |
| `Authority` | `http://keycloak:8080/realms/wegwijs` |
| `IntrospectionEndpoint` | `http://keycloak:8080/realms/wegwijs/protocol/openid-connect/token/introspect` |

**Critical**: The `OIDCAuth.Authority` (used server-side for `RequestAuthorizationCodeTokenAsync`)
must point to the **internal** Docker hostname `keycloak:8080`, while `AuthorizationEndpoint`,
`UserInfoEndPoint`, and `EndSessionEndPoint` (consumed by the Angular browser SPA via
`/v1/security/info`) must point to **external** `localhost:8180`.

**Also**: `WegwijsTokenValidationParameters` validates the JWT issuer. It must accept
`http://localhost:8180/realms/wegwijs` as issuer (Keycloak sets `iss` to the public URL).
`ValidIssuers` must include the external URL even though the API resolves internally — this is
standard for split-horizon setups. The `JwksUri` can remain internal for performance.

#### docker-compose.yml changes

- Add `ASPNETCORE_ENVIRONMENT: Keycloak` to the `api` service (or pass as docker-compose override)
- Ensure Keycloak `--import-realm` flag is already present (it is)
- The `keycloak/` volume mount already points to `./keycloak:/opt/keycloak/data/import`

---

### US2: M2M Demo .NET 8 Minimal API Web UI

#### Architecture

Single-file .NET 8 Minimal API (`demo/m2m/Program.cs`) serving an HTML page with three buttons.
All token acquisition and API calls happen server-side. The browser only receives status codes and
sanitised responses.

#### Flow

1. **Authenticate** button → POST `/demo/authenticate`
   - Server calls Keycloak token endpoint with Client Credentials (`cjmClient` / `secret`)
   - Stores access token in server memory (keyed by session cookie, or simply in-memory singleton
     for demo simplicity)
   - Returns token summary (expiry, scopes) — NOT the raw token
2. **Allowed call** button → POST `/demo/allowed`
   - Uses stored token to call `POST /edit/organisations/{testOvoId}/contacts` on the API
   - Returns HTTP status code + response body excerpt
3. **Forbidden call** button → POST `/demo/forbidden`
   - Uses stored token to call a Keys endpoint: `GET /edit/organisations/{testOvoId}/keys`
   - Returns HTTP status code (expected `403`)

#### Configuration

Environment variables passed via docker-compose:
- `KEYCLOAK_TOKEN_ENDPOINT=http://keycloak:8080/realms/wegwijs/protocol/openid-connect/token`
- `CJM_CLIENT_ID=cjmClient`
- `CJM_CLIENT_SECRET=secret`
- `API_BASE_URL=http://api:80` (internal Docker network)
- `TEST_OVO_ID=OVO002949` (or any existing OVO in the seeded database)

#### docker-compose service

```yaml
m2m-demo:
  build: demo/m2m/
  ports:
    - "5080:8080"
  environment:
    KEYCLOAK_TOKEN_ENDPOINT: http://keycloak:8080/realms/wegwijs/protocol/openid-connect/token
    CJM_CLIENT_ID: cjmClient
    CJM_CLIENT_SECRET: secret
    API_BASE_URL: http://api:80
    TEST_OVO_ID: OVO002949
  depends_on:
    - keycloak
```

---

### US3: Nuxt 3 BFF Demo

#### Architecture

Nuxt 3 server-side rendering with custom server routes for the OIDC flow. No `@sidebase/nuxt-auth`
dependency (adds complexity); instead, minimal custom `h3` server handlers:

- `server/routes/login.ts` — redirect to Keycloak authorization endpoint
- `server/routes/callback.ts` — exchange code for tokens, store in encrypted cookie session
- `server/routes/logout.ts` — clear session + redirect to Keycloak logout
- `server/api/allowed.get.ts` — server-side call to allowed Edit API endpoint (uses session token)
- `server/api/forbidden.get.ts` — server-side call to forbidden Edit API endpoint (uses session token)

Session management: `h3` cookie session with `nuxt-session` or `iron-session` (lightweight,
no DB required). The access token is stored **only** in the encrypted server-side session cookie.
The browser cookie contains an opaque session ID; the token is in the cookie value encrypted with
`SESSION_SECRET`.

Actually, with Nitro's built-in `useSession` / `setCookie`, a simpler approach is: after code
exchange, store the access token in a server-side session using `h3`'s `useSession` with
a secret. This is available out-of-the-box in Nuxt 3.

#### OIDC Flow

1. User visits `/` (unauthenticated) → redirect to `/login`
2. `/login` → redirect to Keycloak with `response_type=code`, `scope=openid profile dv_organisatieregister_cjmbeheerder`, PKCE parameters, `redirect_uri=http://localhost:5090/callback`
3. User logs in at Keycloak → redirect back to `/callback?code=...`
4. `/callback`:
   - Exchange code for tokens at Keycloak token endpoint (server-side)
   - Store access token in session (encrypted cookie, never sent to browser as plain text)
   - Redirect user to `/dashboard`
5. `/dashboard`:
   - SSR: server-side calls to `GET /api/allowed` and `GET /api/forbidden`
   - Renders results in page

#### Token storage strategy

Nuxt 3 `useSession` from `h3`:
```ts
const session = await useSession(event, { password: process.env.SESSION_SECRET! })
session.data.accessToken = tokenResponse.access_token
await session.save()
```
The session is stored as an encrypted JWT cookie (`__session`). The raw `access_token` never
appears in a response body or network request from the browser.

#### Configuration (environment variables)

- `KEYCLOAK_AUTH_URL=http://localhost:8180/realms/wegwijs/protocol/openid-connect/auth`
- `KEYCLOAK_TOKEN_URL=http://keycloak:8080/realms/wegwijs/protocol/openid-connect/token`
- `BFF_CLIENT_ID=nuxt-bff`
- `BFF_CLIENT_SECRET=nuxt-bff-secret`
- `BFF_REDIRECT_URI=http://localhost:5090/callback`
- `API_BASE_URL=http://api:80`
- `SESSION_SECRET=a-32-character-minimum-secret-here`
- `TEST_OVO_ID=OVO002949`

#### docker-compose service

```yaml
nuxt-bff:
  build: demo/nuxt-bff/
  ports:
    - "5090:3000"
  environment:
    KEYCLOAK_AUTH_URL: http://localhost:8180/realms/wegwijs/protocol/openid-connect/auth
    KEYCLOAK_TOKEN_URL: http://keycloak:8080/realms/wegwijs/protocol/openid-connect/token
    BFF_CLIENT_ID: nuxt-bff
    BFF_CLIENT_SECRET: nuxt-bff-secret
    BFF_REDIRECT_URI: http://localhost:5090/callback
    API_BASE_URL: http://api:80
    SESSION_SECRET: demo-session-secret-minimum-32chars
    TEST_OVO_ID: OVO002949
  depends_on:
    - keycloak
```

---

## Implementation Sequence

### Step 1 — Keycloak Realm (US1 foundation, blocks all others)

1. Author `keycloak/realm-export.json`:
   - Realm `wegwijs`, all clients, all client scopes, all users with user attributes
   - Protocol mappers for `iv_wegwijs_rol_3D` (multi-valued User Attribute mapper) and `vo_id`
   - Introspection enabled on `organisation-registry-api` client
2. Verify: `docker-compose up keycloak`, open `http://localhost:8180/realms/wegwijs/.well-known/openid-configuration` returns expected endpoints
3. Author `src/OrganisationRegistry.Api/appsettings.keycloak.json`
4. Start API with `ASPNETCORE_ENVIRONMENT=Keycloak`, run the Angular SPA, test full login flow

### Step 2 — M2M Demo (US2)

1. Author `demo/m2m/m2m.csproj` and `demo/m2m/Program.cs`
2. Author `demo/m2m/Dockerfile`
3. Add `m2m-demo` service to `docker-compose.yml`
4. Test: `docker-compose up keycloak m2m-demo`, open `http://localhost:5080`

### Step 3 — Nuxt BFF (US3)

1. Scaffold `demo/nuxt-bff/` with `nuxt.config.ts`, `package.json`
2. Author server routes (`login`, `callback`, `logout`) and API routes (`allowed`, `forbidden`)
3. Author pages (`index.vue`, `dashboard.vue`)
4. Author `demo/nuxt-bff/Dockerfile` (multi-stage: `node:20-alpine` build → runtime)
5. Add `nuxt-bff` service to `docker-compose.yml`
6. Test: full login flow, verify token not visible in browser Network tab

---

## Key Risks & Mitigations

| Risk | Mitigation |
|---|---|
| Keycloak `iss` claim uses public hostname (`localhost:8180`) but API validates tokens with internal URL | Set `ValidIssuers` in `WegwijsTokenValidationParameters` to include both `http://localhost:8180/realms/wegwijs` and internal URL, or use Keycloak's `frontendUrl` setting to control the `iss` value |
| `iv_wegwijs_rol_3D` multi-valued attribute mapper in realm JSON | Use `"multivalued": true` in the protocol mapper config; Keycloak serialises as JSON array in the token |
| Introspection auth: Keycloak uses `client_credentials` for introspection (not just `client_id` + `client_secret` in body) | `IdentityModel.AspNetCore.OAuth2Introspection` sends Basic auth header by default; Keycloak accepts this — confirmed in research.md |
| Angular SPA has hardcoded `redirect_uri` with `#/oic` fragment | Keycloak client must register this exact URI; hash fragments in redirect URIs require Keycloak `validRedirectUris` to use a wildcard or exact match |
| Nuxt BFF PKCE storage | Use `code_verifier` stored in session before redirect; compare on callback |
