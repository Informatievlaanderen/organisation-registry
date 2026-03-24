# Tasks: Keycloak Demo

**Branch**: `003-keycloak-demo` | **Date**: 2026-03-23 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

## Legend

| Symbol | Meaning |
|---|---|
| `[P]` | Phase number (1–4) |
| `[US1]` | User Story dependency |
| `NEW` | File does not exist; must be created |
| `MOD` | File exists; must be modified |

---

## Phase 1 — Setup: Keycloak Realm + API Config Override

### Keycloak Realm JSON

- [ ] T001 [1] [US1] Create `keycloak/realm-export.json` (NEW) — top-level realm object: `realm: wegwijs`, `enabled: true`, `accessTokenLifespan: 3600`, `registrationAllowed: false`
- [ ] T002 [1] [US1] Add client scopes to `keycloak/realm-export.json`: `dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_orafinbeheerder`, `dv_organisatieregister_info`, `dv_organisatieregister_testclient`
- [ ] T003 [1] [US1] Add client `organisation-registry-local-dev` to `keycloak/realm-export.json`: Authorization Code + PKCE, `secret: a_very=Secr3t*Key`, `validRedirectUris` with both SPA URIs + wildcard, `webOrigins: https://organisatie.dev-vlaanderen.local`, allowed scopes `openid profile dv_organisatieregister_cjmbeheerder`
- [ ] T004 [1] [US1] Add protocol mappers to `organisation-registry-local-dev` in `keycloak/realm-export.json`: `iv_wegwijs_rol_3D` (oidc-usermodel-attribute-mapper, `multivalued: true`, id+access+userinfo token) and `vo_id` (oidc-usermodel-attribute-mapper, `multivalued: false`, id+access+userinfo token)
- [ ] T005 [1] [US1] Add client `cjmClient` to `keycloak/realm-export.json`: Client Credentials, `secret: secret`, `serviceAccountsEnabled: true`, allowed scopes `dv_organisatieregister_cjmbeheerder dv_organisatieregister_info`
- [ ] T006 [1] [US1] Add client `organisation-registry-api` to `keycloak/realm-export.json`: resource server for introspection, `secret: a_very=Secr3t*Key`, `standardFlowEnabled: false`, `directAccessGrantsEnabled: false`, `serviceAccountsEnabled: false`, `publicClient: false` — confidential client used only for introspection (note: `bearerOnly` does not exist in Keycloak 24)
- [ ] T007 [1] [US3] Add client `nuxt-bff` to `keycloak/realm-export.json`: Authorization Code + PKCE, `secret: nuxt-bff-secret`, `validRedirectUris: http://localhost:5090/callback`, `webOrigins: http://localhost:5090`, allowed scopes `openid profile dv_organisatieregister_cjmbeheerder`, same protocol mappers as SPA client
- [ ] T008 [1] [US1] Add test users to `keycloak/realm-export.json` with hashed-on-import passwords and `attributes` (`vo_id`, `iv_wegwijs_rol_3D`): `dev/dev`, `vlimpers/vlimpers`, `decentraalbeheerder/decentraalbeheerder`, `algemeenbeheerder/algemeenbeheerder`, `organen/organen`, `regelgeving/regelgeving`, `cjmbeheerder/cjmbeheerder`
- [ ] T009 [1] [US1] Smoke-test realm import: `docker-compose up keycloak`, verify `http://localhost:8180/realms/wegwijs/.well-known/openid-configuration` returns expected endpoints and all clients/users are present in admin console

### API Config Override

- [ ] T010 [1] [US1] Create `src/OrganisationRegistry.Api/appsettings.keycloak.json` (NEW) — `OIDCAuth` section: `Authority` → `http://keycloak:8080/realms/wegwijs`, `TokenEndPoint` → `/protocol/openid-connect/token`, `AuthorizationIssuer` → `http://localhost:8180/realms/wegwijs`, `JwksUri` → `http://keycloak:8080/realms/wegwijs/protocol/openid-connect/certs`
- [ ] T011 [1] [US1] Add browser-facing OIDC endpoints to `src/OrganisationRegistry.Api/appsettings.keycloak.json`: `AuthorizationEndpoint`, `UserInfoEndPoint`, `EndSessionEndPoint` all pointing to `http://localhost:8180/realms/wegwijs/…`
- [ ] T012 [1] [US1] Add client credentials and JWT settings to `src/OrganisationRegistry.Api/appsettings.keycloak.json`: `ClientId: organisation-registry-local-dev`, `ClientSecret: a_very=Secr3t*Key`, `AuthorizationRedirectUri`, `JwtSharedSigningKey`, `JwtIssuer`, `JwtAudience` (unchanged from development values)
- [ ] T013 [1] [US1] Add `EditApi` section to `src/OrganisationRegistry.Api/appsettings.keycloak.json`: `ClientId: organisation-registry-api`, `ClientSecret: a_very=Secr3t*Key`, `Authority: http://keycloak:8080/realms/wegwijs`, `IntrospectionEndpoint: http://keycloak:8080/realms/wegwijs/protocol/openid-connect/token/introspect`, `FeatureManagement.EditApi: true`

### docker-compose.yml — Keycloak environment fixes

- [ ] T014 [1] [US1] Modify `docker-compose.yml` (MOD) — add `KC_HOSTNAME_URL: http://localhost:8180` and `KC_HOSTNAME_STRICT: "false"` to the `keycloak` service environment (fixes split-horizon `iss` claim)
- [ ] T015 [1] [US1] Document in `demo/README.md` (NEW) how to start the API locally with the Keycloak config: `ASPNETCORE_ENVIRONMENT=Keycloak dotnet run --project src/OrganisationRegistry.Api` — the API runs outside Docker, `appsettings.keycloak.json` is loaded by the environment name

### Phase 1 acceptance verification

- [ ] T016 [1] [US1] Manual test: start `docker-compose up keycloak` + API, open Angular SPA, click "inloggen", log in as `dev/dev`, verify SPA receives custom JWT (Network tab shows 200 from `/v1/security/exchange`) — covers SC-002

---

## Phase 2 — US1: Angular SPA werkt met Keycloak

> Phase 2 tasks are the integration-level verification tasks that confirm US1 acceptance scenarios end-to-end once Phase 1 files are in place.

- [ ] T017 [2] [US1] Verify AC1: call `GET /v1/security/info` and confirm response body contains OIDC endpoints pointing to `localhost:8180` (not `localhost:5050`)
- [ ] T018 [2] [US1] Verify AC2: in Angular SPA click "inloggen", confirm browser redirects to `http://localhost:8180/realms/wegwijs/protocol/openid-connect/auth` (check Network tab Location header)
- [ ] T019 [2] [US1] Verify AC3: complete Keycloak login as `dev/dev`, confirm Angular SPA callback calls `GET /v1/security/exchange?code=…&verifier=…`
- [ ] T020 [2] [US1] Verify AC4: confirm `POST /v1/security/exchange` returns a custom JWT that contains `iv_wegwijs_rol_3D` and `vo_id` claims (decode JWT in browser console or jwt.io)
- [ ] T021 [2] [US1] Verify AC5: navigate to a protected screen in the SPA and confirm subsequent API calls use `Authorization: Bearer <custom_jwt>` and return 200 OK
- [ ] T022 [2] [US1] Verify edge case — introspection: start `docker-compose up keycloak` only (no Duende), confirm M2M introspection endpoint `http://keycloak:8080/realms/wegwijs/protocol/openid-connect/token/introspect` responds to a test `POST` with valid client credentials

---

## Phase 3 — US2: M2M .NET Minimal API Demo in Docker

### Project files

- [ ] T023 [3] [US2] Create `demo/m2m/m2m.csproj` (NEW) — .NET 8 project, `<TargetFramework>net8.0</TargetFramework>`, `Microsoft.AspNetCore.App` framework reference, no extra NuGet packages
- [ ] T024 [3] [US2] Create `demo/m2m/Program.cs` (NEW) — `GET /` endpoint: serves inline HTML page with three buttons (Authenticate, Allowed call, Forbidden call) and a `<pre id="result">` block with JS `fetch` calls to the three POST endpoints
- [ ] T025 [3] [US2] Add `POST /demo/authenticate` to `demo/m2m/Program.cs`: calls Keycloak token endpoint with Client Credentials (`CJM_CLIENT_ID` / `CJM_CLIENT_SECRET`), stores `access_token` in static in-memory holder, returns JSON `{ expires_in, scope }` — raw token NOT returned
- [ ] T026 [3] [US2] Add `POST /demo/allowed` to `demo/m2m/Program.cs`: uses stored token to call `POST /edit/organisations/{TEST_OVO_ID}/contacts` on `API_BASE_URL` with an intentionally minimal/invalid body — auth is checked before model validation so 403 proves access, 400 proves allowed (not 401/403); returns JSON `{ status, body }`; returns 400 with message if not yet authenticated
- [ ] T027 [3] [US2] Add `POST /demo/forbidden` to `demo/m2m/Program.cs`: uses stored token to call `POST /edit/organisations/{TEST_OVO_ID}/keys` on `API_BASE_URL` (policy `Keys` requires `dv_organisatieregister_orafinbeheerder` — cjmClient lacks this); returns JSON `{ status, body }` — expected status 403; returns 400 with message if not yet authenticated
- [ ] T028 [3] [US2] Read environment variables in `demo/m2m/Program.cs`: `KEYCLOAK_TOKEN_ENDPOINT`, `CJM_CLIENT_ID`, `CJM_CLIENT_SECRET`, `API_BASE_URL`, `TEST_OVO_ID` — fail fast with descriptive error if any are missing; `API_BASE_URL` points to `http://host.docker.internal:<api_port>` so the container reaches the locally running API

### Dockerfile

- [ ] T029 [3] [US2] Create `demo/m2m/Dockerfile` (NEW) — multi-stage: stage 1 `mcr.microsoft.com/dotnet/sdk:8.0` runs `dotnet publish -c Release -o /app/publish`; stage 2 `mcr.microsoft.com/dotnet/aspnet:8.0` copies published output, `EXPOSE 8080`, `ENTRYPOINT ["dotnet", "m2m.dll"]`

### docker-compose.yml splits

- [ ] T030 [3] [US2] Create `demo/docker-compose.yml` (NEW) — standalone compose file for demo services only: `m2m-demo` service: `build: ./m2m/`, `ports: 5080:8080`, environment variables `KEYCLOAK_TOKEN_ENDPOINT` (pointing to `http://host.docker.internal:8180/realms/wegwijs/…`), `CJM_CLIENT_ID`, `CJM_CLIENT_SECRET`, `API_BASE_URL: http://host.docker.internal:<api_port>`, `TEST_OVO_ID`; add `extra_hosts: host.docker.internal:host-gateway` for Linux compatibility

### Phase 3 acceptance verification

- [ ] T031 [3] [US2] Verify AC5/SC-006: `docker-compose up keycloak m2m-demo`, open `http://localhost:5080`, confirm page loads with three buttons
- [ ] T032 [3] [US2] Verify AC1/SC-003: click "Authenticate", confirm page shows token summary (expiry + scopes) — raw token must NOT be visible in browser Network tab response body
- [ ] T033 [3] [US2] Verify AC2/SC-003: click "Allowed call", confirm page shows HTTP status 2xx
- [ ] T034 [3] [US2] Verify AC3/SC-003: click "Forbidden call", confirm page shows exactly `403 Forbidden`
- [ ] T035 [3] [US2] Verify AC4: without clicking "Authenticate" first, click "Allowed call" or "Forbidden call", confirm error message is shown

---

## Phase 4 — US3: Nuxt 3 BFF Demo in Docker

### Project scaffolding

- [ ] T036 [4] [US3] Create `demo/nuxt-bff/package.json` (NEW) — `nuxt: ^3.x` dependency, `scripts: { build: "nuxt build", dev: "nuxt dev" }`, no extra auth libraries
- [ ] T037 [4] [US3] Create `demo/nuxt-bff/nuxt.config.ts` (NEW) — `ssr: true`, `runtimeConfig` exposing server-only env vars (`keycloakAuthUrl`, `keycloakTokenUrl`, `bffClientId`, `bffClientSecret`, `bffRedirectUri`, `apiBaseUrl`, `sessionSecret`, `testOvoId`), `nitro.preset: 'node-server'`

### Server routes — OIDC flow

- [ ] T038 [4] [US3] Create `demo/nuxt-bff/server/routes/login.get.ts` (NEW) — generates PKCE `code_verifier` + `code_challenge` (SHA-256), stores `code_verifier` in session via `useSession`, redirects to Keycloak authorization endpoint with `response_type=code`, `scope=openid profile dv_organisatieregister_cjmbeheerder`, `code_challenge_method=S256`, `redirect_uri`
- [ ] T039 [4] [US3] Create `demo/nuxt-bff/server/routes/callback.get.ts` (NEW) — retrieves `code_verifier` from session, exchanges authorization `code` for tokens at `KEYCLOAK_TOKEN_URL` (server-side, internal Docker URL), stores `access_token` in encrypted session using Nitro `useSession`, redirects to `/dashboard`; `access_token` must NOT appear in any response body
- [ ] T040 [4] [US3] Create `demo/nuxt-bff/server/routes/logout.get.ts` (NEW) — clears server session, redirects to Keycloak logout endpoint (`/realms/wegwijs/protocol/openid-connect/logout`)

### Server API routes — proxied calls

- [ ] T041 [4] [US3] Create `demo/nuxt-bff/server/api/allowed.get.ts` (NEW) — reads `access_token` from session, calls `POST /edit/organisations/{TEST_OVO_ID}/contacts` on `API_BASE_URL` with minimal body and `Authorization: Bearer` (auth checked before model validation — 400 means allowed, not 403); returns JSON `{ status, data }`; returns 401 if no session token
- [ ] T042 [4] [US3] Create `demo/nuxt-bff/server/api/forbidden.get.ts` (NEW) — reads `access_token` from session, calls `POST /edit/organisations/{TEST_OVO_ID}/keys` on `API_BASE_URL` with `Authorization: Bearer` (expects 403, policy `Keys` requires `dv_organisatieregister_orafinbeheerder`); returns JSON `{ status }`; returns 401 if no session token

### Auth middleware

- [ ] T043 [4] [US3] Create `demo/nuxt-bff/server/middleware/auth.ts` (NEW) — Nitro server middleware: checks `access_token` in session for all non-public routes; bypasses for `/login`, `/callback`, `/logout`, and `/api/*`; redirects unauthenticated requests to `/login`

### Pages

- [ ] T044 [4] [US3] Create `demo/nuxt-bff/pages/index.vue` (NEW) — landing page: "Log in via Keycloak" button linking to `/login`; if authenticated (session check), redirects to `/dashboard`
- [ ] T045 [4] [US3] Create `demo/nuxt-bff/pages/dashboard.vue` (NEW) — protected page: shows user greeting, allowed call result (`useFetch('/api/allowed')` — status + data excerpt), forbidden call result (`useFetch('/api/forbidden')` — shows `403 Forbidden` as expected), logout link to `/logout`

### Dockerfile

- [ ] T046 [4] [US3] Create `demo/nuxt-bff/Dockerfile` (NEW) — multi-stage: stage 1 `node:20-alpine` runs `npm ci && npm run build`; stage 2 `node:20-alpine` copies `.output/`, `EXPOSE 3000`, `CMD ["node", ".output/server/index.mjs"]`

### demo/docker-compose.yml — nuxt-bff service

- [ ] T047 [4] [US3] Add `nuxt-bff` service to `demo/docker-compose.yml` (MOD) — `build: ./nuxt-bff/`, `ports: 5090:3000`, environment variables `KEYCLOAK_AUTH_URL` (external, `http://localhost:8180/…`), `KEYCLOAK_TOKEN_URL` (internal, `http://host.docker.internal:8180/…`), `BFF_CLIENT_ID`, `BFF_CLIENT_SECRET`, `BFF_REDIRECT_URI: http://localhost:5090/callback`, `API_BASE_URL: http://host.docker.internal:<api_port>`, `SESSION_SECRET`, `TEST_OVO_ID`; add `extra_hosts: host.docker.internal:host-gateway` for Linux compatibility

### Phase 4 acceptance verification

- [ ] T048 [4] [US3] Verify AC6/SC-006: `docker-compose up keycloak nuxt-bff` (+ API), open `http://localhost:5090`, confirm page loads with login button
- [ ] T049 [4] [US3] Verify AC1: click "inloggen", confirm browser redirects to `http://localhost:8180/realms/wegwijs/protocol/openid-connect/auth`
- [ ] T050 [4] [US3] Verify AC2/SC-004: complete login as `dev/dev`, confirm redirect to `/dashboard`; open browser Network tab and verify `access_token` does NOT appear in any response body or cookie value in plain text
- [ ] T051 [4] [US3] Verify AC3/SC-004: dashboard shows allowed call result with 2xx status
- [ ] T052 [4] [US3] Verify AC4/SC-004: dashboard shows forbidden call result with `403 Forbidden`
- [ ] T053 [4] [US3] Verify AC5: clear session cookie and visit `http://localhost:5090/dashboard` directly, confirm automatic redirect to Keycloak login
- [ ] T054 [4] [US3] Verify SC-004 security: inspect browser DevTools Network tab during entire login + dashboard flow, confirm `access_token` string is never present in any XHR/fetch response

---

## Summary

| Phase | Scope | Tasks |
|---|---|---|
| 1 | Setup: realm JSON + appsettings.keycloak.json + Keycloak env in docker-compose.yml + demo/README.md | T001–T016 |
| 2 | US1 acceptance verification: Angular SPA with Keycloak | T017–T022 |
| 3 | US2: M2M .NET Minimal API demo + demo/docker-compose.yml | T023–T035 |
| 4 | US3: Nuxt 3 BFF demo + demo/docker-compose.yml uitbreiden | T036–T054 |

**Compose structuur**:
- `docker-compose.yml` — Keycloak (poort 8180) + bestaande infra (mssql, es, wiremock, acm)
- `demo/docker-compose.yml` — M2M demo (poort 5080) + Nuxt BFF (poort 5090); beide bereiken de lokaal draaiende API via `host.docker.internal`
- API + Angular SPA draaien lokaal: `ASPNETCORE_ENVIRONMENT=Keycloak dotnet run` + `npm run start`

**Total tasks**: 54
