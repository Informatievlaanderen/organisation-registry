# 003 Keycloak Demo — Onepager

**Branch**: `003-keycloak-demo` | **Status**: in progress | **Commits**: 49 ahead of main

---

## Doel

Aantonen dat de Organisation Registry API kan werken met **Keycloak als identity provider**,
ter vervanging van de huidige Duende IdentityServer (poort 5050). Drie use cases worden gedemonstreerd:

1. **US1** — Bestaande Angular SPA logt in via Keycloak (Authorization Code + PKCE → custom JWT)
2. **US2** — Machine-to-machine: .NET console met Client Credentials via Keycloak
3. **US3** — Nuxt 3 BFF: gebruikerslogin met server-side RFC 8693 token exchange

---

## Wat is gebouwd

### Keycloak Realm (`keycloak/realm-export.json`)

Volledig geconfigureerde realm `wegwijs`:
- Clients: `organisation-registry-local-dev` (PKCE), `cjmClient` (M2M), `nuxt-bff` (BFF), `organisation-registry-api` (introspection resource)
- Scopes: `dv_organisatieregister_cjmbeheerder`, `orafinbeheerder`, `info`, `testclient`
- Protocol mappers: `iv_wegwijs_rol_3D` (multivalued) en `vo_id` op id+access token
- Testgebruikers: `dev/dev`, `vlimpers`, `cjmbeheerder`, `algemeenbeheerder`, etc.
- Keycloak 26 (geüpgraded van 24 voor RFC 8693 token exchange support)

### API configuratie

- `src/OrganisationRegistry.Api/appsettings.development.json` — Keycloak authority, JwksUri, introspection endpoint
- `BffApi` scheme toegevoegd — OAuth2 introspection voor tokens uitgewisseld door de Nuxt BFF
- `BffClaimsTransformation` — mapt Keycloak-claims (`given_name`, `family_name`) naar de verwachte claims
- `OrganisationRegistryAuthorizeAttribute` — ondersteunt nu zowel `EditApi` als `BffApi` scheme
- `ClaimsPrincipalSelector` — selecteert het juiste principal op basis van authenticatiescheme

### US2: M2M Demo (`demos/m2m/`)

.NET 6 console app die via `orafinClient` (Client Credentials) een token ophaalt bij Keycloak
en vervolgens een _allowed_ en een _forbidden_ call doet naar de Edit API.

### US3: Nuxt BFF (`demo/nuxt-bff/`)

Nuxt 3 SSR applicatie met volledige OIDC-flow:
- `GET /login` → PKCE challenge genereren, redirect naar Keycloak
- `GET /callback` → authorization code inwisselen + **RFC 8693 token exchange** (nuxt-bff token → API-scoped token)
- Token opgeslagen in encrypted session cookie, **nooit blootgesteld aan de browser**
- `GET /api/allowed` en `GET /api/forbidden` — server-side calls naar Edit API met het uitgewisselde token
- Dashboard toont beide resultaten (2xx vs 403)

### K8s / Tilt lokale dev omgeving

Volledige k3d dev stack met Tiltfile:
- **Infrastructure**: SQL Server, Keycloak, Seq (logging), otel-collector (OTLP → Seq)
- **Applications**: API, Angular UI, Vue/Nuxt UI, Nuxt BFF
- **Demo**: M2M, seed
- Subdomain-based routing via Traefik ingress: `*.localhost:9080`
  - `api.localhost:9080`, `ui.localhost:9080`, `app.localhost:9080`, `keycloak.localhost:9080`
- `k3d.config.yaml` met relaxed disk pressure thresholds
- `dev-setup.sh` bootstrap script voor fresh clone

### Seed (`demos/seed/`)

.NET 6 console app die de database vult met:
- Parameter types (classificaties, contacttypes, etc.)
- Testorganisaties (Vlimpers)
- Idempotent via GET-check voor aanmaken

### Observability

- `otel-collector-config.yaml` tussen API en Seq voor OTLP ingestion
- Seq bereikbaar via `seq.localhost:9080`

---

## Technische knelpunten opgelost

| Probleem | Oplossing |
|---|---|
| Keycloak `iss` claim split-horizon (intern vs extern) | `InternalAuthorityOverride` in appsettings voor server-side token calls |
| `RoleClaimType` mismatch → 403 op `/v1/events` | `BffClaimsTransformation` zonder `AuthenticationType` guard |
| Angular login werkt niet met Keycloak | `LegacySelfMintedToken` scheme vervangen door `Bearer` in authorize attribute |
| CORS met `withCredentials` werkt niet met `AllowAnyOrigin` | Terug naar `AllowSpecificOrigin` met expliciete subdomain origins |
| k3d registry niet bereikbaar op niet-localhost omgevingen | Registry host gefixed op `0.0.0.0` |
| Nuxt BFF session slaat id_token niet op | Session uitgebreid, logout gebruikt `URLSearchParams` |

---

## Staat van de branch

| Component | Status |
|---|---|
| Keycloak realm export | Compleet |
| US1: Angular SPA + Keycloak | Werkend |
| US2: M2M demo | Werkend |
| US3: Nuxt BFF + token exchange | Werkend |
| K8s/Tilt dev omgeving | Werkend |
| Seed | Werkend |
| Unit tests BffClaimsTransformation | Aanwezig |
| `002-auth-rework` branch | Leeg — geen commits |

---

## Volgende stappen

### Korte termijn — demo afronden

- [ ] **Integratie tests** voor de nieuwe API auth schemes (`BffApi`, `EditApi` met Keycloak)
  — nu alleen unit tests voor `BffClaimsTransformation`
- [ ] **Token refresh** in de Nuxt BFF — huidige implementatie heeft geen refresh flow;
  na verlopen van het access token moet de gebruiker opnieuw inloggen
- [ ] **README** voor de demo — `demo/README.md` ontbreekt (was gepland als T015);
  how-to voor `dev-setup.sh`, Tilt, docker-compose
- [ ] **Secrets** — harde secrets in `k3d.config.yaml`, `demo/k8s/secrets.yaml`, `.env`-bestanden;
  voor een échte demo-omgeving moeten die via `.env.example` + gitignore afgeschermd worden

### Middellange termijn — richting productie

- [ ] **`002-auth-rework`** — de eigenlijke rework van het auth-mechanisme in de bestaande API.
  De demo bewijst het concept; nu moet de vraag beantwoord worden: _gaan we Keycloak ook in
  productie inzetten als vervanging voor ACM/IDM + Duende?_ Of blijft dit een lokale demo?

- [ ] **ACM/IDM integratie** — het ACM/IDM integratiedossier (`ACM-IDM_IV-Wegwijs_organisatie_Integratiedossier_OIDC v4.pdf`)
  ligt in de root. Is Keycloak hier een tussenlaag (identity broker naar ACM/IDM) of
  een volledige vervanging? Dit is de architectuurvraag die de branch `002-auth-rework` zou moeten beantwoorden.

- [ ] **Token exchange in productie** — RFC 8693 vereist Keycloak 26. Als ACM/IDM de IDP blijft,
  moet bepaald worden of token exchange daar ook supported is, of dat de BFF dan anders werkt.

- [ ] **Keycloak in k8s productie-setup** — huidige `demo/k8s/keycloak.yaml` is niet production-ready
  (geen HA, geen externe DB, geen TLS). Aparte track nodig als Keycloak de definitieve IDP wordt.

### Architectuurvraag om te beantwoorden

> Wat is de eindbestemming van dit werk?
>
> **Optie A**: De demo toont aan dat Keycloak _kan_ werken → de input voor de beslissing of
> ACM/IDM vervangen wordt door Keycloak in productie. Branch `002-auth-rework` voert dan
> de echte migratie door.
>
> **Optie B**: Keycloak blijft een lokale dev-tool die ACM/IDM spiegelt → de demo-branch wordt
> gemerged als dev-tooling, en `002-auth-rework` raakt the auth-abstraction zodat de API
> makkelijker van IDP kan wisselen (interface-based, config-driven).
>
> **Optie C**: De BFF is het eigenlijke product — de Angular SPA wordt vervangen door een
> BFF-gebaseerde architectuur (Nuxt 3) die token exchange centraal stelt.

---

## Teststatus

### Wat op deze branch getest is

#### `BffClaimsTransformationTests` — 10 unit tests
**Locatie**: `test/OrganisationRegistry.UnitTests/Security/BffClaimsTransformationTests.cs`
**Klasse onder test**: `BffClaimsTransformation` — de ASP.NET `IClaimsTransformation` die
claims uit een OAuth2 introspection response (van de Nuxt BFF) mapt naar de interne claims
die `[OrganisationRegistryAuthorize]` en `SecurityService` verwachten.

| Test | Wat het verifieert |
|---|---|
| `WhenIdentityAlreadyHasClaimTypesGivenName_IsNotTransformed` | Bearer-identity (custom JWT) wordt niet aangeraakt — guard tegen double-transformation |
| `WhenNoIdentityHasGivenName_PrincipalIsReturnedUnchanged` | Principal zonder `given_name` wordt onveranderd teruggegeven |
| `MapsGivenNameAndFamilyNameToClaimTypes` | `given_name`/`family_name` (JWT short form) → `ClaimTypes.GivenName`/`ClaimTypes.Surname` |
| `MapsIvWegwijsRolClaimToClaimTypesRole` (×6 rollen) | Elke ACM/IDM rol (`AlgemeenBeheerder`, `VlimpersBeheerder`, etc.) wordt correct gemapt naar `ClaimTypes.Role` |
| `IsInRole_ReturnsTrueAfterTransformation` (×6 rollen) | **Regressietest** voor de `RoleClaimType`-mismatch bug: na transformatie geeft `IsInRole()` `true` terug. Reproduceert de bug door de `ClaimsIdentity` exact te construeren zoals de OAuth2Introspection library dat doet na de fix in `Startup.cs` |
| `WhenAlgemeenBeheerder_OtherRolesAreNotAdded` | AlgemeenBeheerder is een superrol: andere rollen worden weggelaten als deze aanwezig is |
| `DecentraalBeheerder_AddsOrganisationClaimsForEachOvo` | Per OVO-nummer wordt een `iv_wegwijs_organisatie`-claim toegevoegd |
| `WhenNoMatchingRolePrefix_NoRoleClaimsAdded` | Rollen van andere systemen (zonder `wegwijs`-prefix) leveren géén rollen op |
| `WhenNoIvWegwijsRolClaims_NoRoleClaimsAdded` | Identity zonder rollen krijgt geen role-claims |
| `MultipleNonAlgemeenRoles_AreAllAdded` | Combinatie van niet-superrollen worden allemaal toegevoegd |

**Testpatroon**: pure unit tests, geen mocks of externe dependencies. De helper
`IntrospectionIdentity()` bouwt een `ClaimsIdentity` met `RoleClaimType = ClaimTypes.Role`
— precies zoals `OAuth2IntrospectionOptions.RoleClaimType` is ingesteld na de bugfix in `Startup.cs`.

---

### Wat al bestond (op main, niet gewijzigd)

#### `OrganisationRegistryTokenBuilderTests`
**Locatie**: `test/OrganisationRegistry.UnitTests/Security/OrganisationRegistryTokenBuilder.cs`
Verifieert dat `OrganisationRegistryTokenBuilder.ParseRoles()` ACM/IDM-rolclaims correct parst
naar interne `ClaimTypes.Role`-claims — dezelfde rollogica als `BffClaimsTransformation`,
maar dan voor de Bearer/custom JWT flow (US1, Angular SPA).

#### `OrganisationSecurityCacheTests`
**Locatie**: `test/OrganisationRegistry.UnitTests/Security/OrganisationSecurityCacheTests.cs`
Tests voor de security cache die bepaalt welke organisaties een DecentraalBeheerder mag beheren.

#### EditApi integratie tests (`[EnvVarIgnoreFact]`)
**Locatie**: `test/OrganisationRegistry.Api.IntegrationTests/EditApi/`
Draaien tegen een live API+Duende stack (via `ApiFixture`), gecontroleerd door env vars.
Dekken voor de M2M flow:
- Unauthorized zonder Bearer
- Forbidden als verkeerd scope (`AsOrafinBeheerder` op contacts-endpoint)
- Correct gedrag bij juist scope (`AsCJM` op contacts, bankaccounts, classifications, keys)

Deze tests zijn **niet aangepast** op deze branch en draaien nog tegen de Duende IdentityServer.

---

### Wat niet getest is (gaps)

| Component | Type test dat ontbreekt |
|---|---|
| `OrganisationRegistryAuthorizeAttribute` | Unit test: nieuwe `BffApi`-scheme branch, `ClaimsPrincipalSelector`-integratie |
| `ConfigureClaimsPrincipalSelectorMiddleware` | Unit test: juiste principal geselecteerd per scheme |
| `BffApi` scheme end-to-end | Integratie test: request met introspection token → 200/403 (analoog aan bestaande EditApi tests) |
| `SecurityController` (Keycloak) | Integratie test: `/v1/security/exchange` met Keycloak authorization code → custom JWT |
| Token exchange (RFC 8693) | Geen geautomatiseerde test — alleen manueel gevalideerd via de Nuxt BFF demo |
| Nuxt BFF server routes | Geen TS/JS tests (login, callback, token storage) |
| Seed idempotentie | Geen test — impliciet gevalideerd door de seed-container te herdraaien |

---

_Gegenereerd: 2026-04-15 | Branch: `003-keycloak-demo` | 49 commits ahead of main_
