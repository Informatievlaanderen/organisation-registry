# Feature Specification: Keycloak Demo — Identity Provider, M2M en BFF

**Feature Branch**: `002-keycloak-demo`  
**Created**: 2026-03-23  
**Status**: Draft  
**Input**: User description: "Keycloak demo: laat Keycloak (24.0, poort 8180) werken als identity provider voor de bestaande Angular SPA flow (authorisation code + PKCE → token exchange bij de API → custom JWT). Daarna een machine-to-machine demo: een kleine .NET Minimal API web UI (in Docker, bereikbaar via browser) met 3 knoppen — "Authenticate" (haalt client credentials token op via cjmClient), "Allowed call" (roept een EditApi endpoint op waar cjmClient rechten voor heeft), "Forbidden call" (roept een endpoint op waar cjmClient geen rechten voor heeft, verwacht 403). Ten slotte een Nuxt 3 BFF demo: gebruiker logt in via Keycloak (authorization code flow), de BFF doet server-side token exchange voor een API-scoped token, en toont een pagina met een "allowed" call en een "forbidden" call naar de Organisation Registry API. Alles draait lokaal via docker-compose."

---

## Context & Architectuurkader

### Huidig auth-mechanisme

De Organisation Registry API hanteert een **tweeledig authenticatiemechanisme**:

1. **Angular SPA (gebruikers)** — Authorization Code + PKCE flow via ACM/IDM (Duende IdentityServer, poort 5050):
   - Angular roept `/v1/security/info` op om OIDC-instellingen op te halen
   - Redirect naar IdentityServer voor login
   - Callback met `code` → API `/v1/security/exchange?code=...&verifier=...`
   - API doet token exchange bij IdentityServer, bouwt een **eigen custom JWT** (gesigneerd met `JwtSharedSigningKey`)
   - Alle volgende requests dragen dit custom JWT als `Bearer`-token
   - Validatie: `JwtBearerDefaults.AuthenticationScheme` met `OrganisationRegistryTokenValidationParameters`

2. **Edit API (machine-to-machine)** — OAuth2 introspection via `AddOAuth2Introspection`:
   - Clients (`cjmClient`, `orafinClient`, `testClient`) halen een access token op via Client Credentials
   - De API valideert het token live via de introspection endpoint van de IdentityServer
   - Autorisatie via `scope`-claims: `dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_orafinbeheerder`, etc.
   - Controllers zijn versierd met `[Authorize(AuthenticationSchemes = AuthenticationSchemes.EditApi, Policy = PolicyNames.X)]`

### Huidige IdentityServer-configuratie (Duende, poort 5050)

Relevante clients in `src/IdentityServer/Config.cs`:
- `organisation-registry-local-dev` — Authorization Code + PKCE, gebruikt door Angular SPA
- `cjmClient` — Client Credentials, scopes: `dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_info`
- `orafinClient` — Client Credentials, scope: `dv_organisatieregister_orafinbeheerder`
- `testClient` — Client Credentials, alle scopes

API Resource: `organisation-registry-local-dev` met secret `a_very=Secr3t*Key`, voor introspection.

### Keycloak (poort 8180)

Al geconfigureerd in `docker-compose.yml`:
- Image: `quay.io/keycloak/keycloak:24.0`
- `--import-realm` flag: leest realm-configuratie uit `./keycloak/` volume
- `./keycloak/` is momenteel leeg — de realm moet nog aangemaakt worden

### Edit API — toegangsmodel

| Scope | Endpoints toegankelijk |
|---|---|
| `dv_organisatieregister_cjmbeheerder` | Contacts, Classifications, BankAccounts, Keys (gedeeld met Orafin) |
| `dv_organisatieregister_orafinbeheerder` | Keys |
| `dv_organisatieregister_info` | Informatieve endpoints (read-only) |

**Toegestaan voor `cjmClient`**: endpoints met policy `OrganisationContacts`, `OrganisationClassifications`, `BankAccounts`  
**Verboden voor `cjmClient`**: endpoints met policy `Keys` (vereist `dv_organisatieregister_orafinbeheerder` of `dv_organisatieregister_testclient`)

---

## User Scenarios & Testing *(mandatory)*

### User Story 1 — Keycloak als Identity Provider voor Angular SPA (Priority: P1)

Een ontwikkelaar kan de bestaande Angular SPA laten draaien met Keycloak als identity provider in plaats van de Duende IdentityServer, zonder de Angular-broncode aan te passen. De flow is: Angular → Keycloak (authorization code + PKCE) → API `/v1/security/exchange` → custom JWT.

**Why this priority**: Dit is de fundering. Keycloak moet draaien en werkende OIDC-endpoints blootstellen die de bestaande `SecurityController.ExchangeCode` flow ondersteunen. Zonder dit werken de andere demo's ook niet.

**Independent Test**: Start `docker-compose up keycloak` + API. Ga naar de Angular SPA, klik op "inloggen", log in als een testgebruiker in Keycloak, en verifieer dat de SPA een custom JWT ontvangt en ingelogd is.

**Acceptance Scenarios**:

1. **Given** Keycloak draait op poort 8180 met een geconfigureerde realm, **When** de Angular SPA `/v1/security/info` opvraagt, **Then** geeft de API OIDC-configuratie terug die naar Keycloak-endpoints wijst (poort 8180).

2. **Given** de gebruiker klikt op "inloggen" in de Angular SPA, **When** de PKCE-flow start, **Then** wordt de browser geredirect naar `http://localhost:8180/realms/wegwijs/protocol/openid-connect/auth`.

3. **Given** de gebruiker logt in als `dev` / `dev` in Keycloak, **When** de callback arriveert met een `code`, **Then** roept de Angular SPA `/v1/security/exchange?code=...&verifier=...` aan.

4. **Given** de API ontvangt een geldige authorization code, **When** de API de code uitwisselt bij Keycloak, **Then** geeft de API een custom JWT terug dat de wegwijs-rollen van de gebruiker bevat.

5. **Given** de SPA beschikt over het custom JWT, **When** de gebruiker navigeert naar een beveiligd scherm, **Then** worden API-aanroepen uitgevoerd met `Authorization: Bearer <custom_jwt>` en zijn ze succesvol (200 OK).

---

### User Story 2 — Machine-to-Machine Demo: .NET Minimal API Web UI (Priority: P2)

Een ontwikkelaar kan via een web UI in de browser demonstreren hoe een machine-to-machine (Client Credentials) flow werkt met Keycloak. De UI heeft drie knoppen die de volledige M2M-cyclus tonen.

**Why this priority**: De M2M-demo illustreert het Edit API-toegangsmodel (scope-based autorisatie). Ze bouwt verder op de Keycloak-configuratie van US1 maar is technisch onafhankelijk van de Angular SPA.

**Independent Test**: Start `docker-compose up keycloak m2m-demo`. Open `http://localhost:<poort>` in de browser. Klik de drie knoppen in volgorde en verifieer de resultaten.

**Acceptance Scenarios**:

1. **Given** de M2M web UI draait in Docker, **When** de gebruiker op "Authenticate" klikt, **Then** haalt de UI een Client Credentials access token op van Keycloak als `cjmClient` en toont het token (of een samenvatting) op de pagina.

2. **Given** de gebruiker heeft een geldig token via "Authenticate", **When** op "Allowed call" geklikt wordt, **Then** roept de UI een EditApi endpoint aan waar `cjmClient` rechten voor heeft (bijv. `POST /edit/organisations/{id}/contacts`) en toont de HTTP-statuscode `201` of `200`.

3. **Given** de gebruiker heeft een geldig token via "Authenticate", **When** op "Forbidden call" geklikt wordt, **Then** roept de UI een EditApi endpoint aan waarvoor `cjmClient` géén rechten heeft (bijv. een endpoint met policy `Keys` dat `dv_organisatieregister_orafinbeheerder` vereist) en toont de HTTP-statuscode `403`.

4. **Given** nog niet geauthenticeerd, **When** op "Allowed call" of "Forbidden call" geklikt wordt, **Then** toont de UI een foutmelding dat eerst geauthenticeerd moet worden.

5. **Given** de UI draait in Docker, **When** de container start, **Then** is de UI bereikbaar via de browser op een vaste poort (bijv. `http://localhost:5080`).

---

### User Story 3 — Nuxt 3 BFF Demo: Gebruikerslogin met server-side token exchange (Priority: P3)

Een ontwikkelaar kan via een Nuxt 3 BFF demonstreren hoe een interactieve gebruikersstroom werkt waarbij de BFF server-side een API-scoped token ophaalt na login. De pagina toont resultaten van een "allowed" en een "forbidden" API-aanroep.

**Why this priority**: De BFF-demo toont het geavanceerde patroon van server-side token management. Ze bouwt voort op de Keycloak-configuratie van US1 en vereist een draaiende Organisation Registry API.

**Independent Test**: Start `docker-compose up keycloak m2m-demo nuxt-bff` (+ API). Open `http://localhost:<poort>` in de browser. Klik "inloggen", log in via Keycloak, en verifieer dat de pagina de resultaten van de allowed en forbidden call toont.

**Acceptance Scenarios**:

1. **Given** de Nuxt 3 BFF draait in Docker, **When** de gebruiker op "inloggen" klikt, **Then** wordt de browser geredirect naar Keycloak voor authorization code flow.

2. **Given** de gebruiker heeft succesvol ingelogd via Keycloak, **When** de callback arriveert bij de BFF, **Then** voert de BFF **server-side** een token exchange uit voor een API-scoped access token (scope `dv_organisatieregister_cjmbeheerder`) en slaat deze op in de sessie (nooit blootgesteld aan de browser).

3. **Given** de gebruiker is ingelogd en de BFF heeft een geldig API-scoped token, **When** de pagina laadt, **Then** doet de BFF server-side een "allowed" API-aanroep (bijv. een GET-endpoint op EditApi waar `cjmClient` rechten voor heeft) en toont het resultaat.

4. **Given** de gebruiker is ingelogd en de BFF heeft een geldig API-scoped token, **When** de pagina laadt, **Then** doet de BFF server-side een "forbidden" API-aanroep (bijv. een endpoint waarvoor `cjmClient` geen rechten heeft) en toont de 403-statuscode als verwacht resultaat.

5. **Given** de gebruiker is **niet** ingelogd, **When** de beveiligde pagina bezocht wordt, **Then** redirect de BFF automatisch naar Keycloak voor login.

6. **Given** de BFF draait in Docker, **When** de container start, **Then** is de BFF bereikbaar via de browser op een vaste poort (bijv. `http://localhost:5090`).

---

### Edge Cases

- **Keycloak claim-mapping**: Keycloak gebruikt andere claim-namen dan Duende IdentityServer. De `OrganisationRegistryTokenBuilder` leest `iv_wegwijs_rol_3D`- en `vo_id`-claims. Deze moeten via een Keycloak Mapper op het `id_token` gezet worden, anders zijn gebruikersrollen leeg.
- **PKCE verifier opslag**: De Angular SPA slaat de PKCE `code_verifier` op in `localStorage`. Dit werkt alleen als de `redirect_uri` exact overeenkomt met de geconfigureerde URI in Keycloak (inclusief `#/oic` hash-fragment).
- **Introspection endpoint**: De Edit API valideert M2M-tokens via OAuth2 introspection. Keycloak ondersteunt introspection maar het endpoint-pad verschilt van Duende (`/realms/{realm}/protocol/openid-connect/token/introspect`). Dit pad moet geconfigureerd worden in `EditApi:IntrospectionEndpoint`.
- **CORS op Keycloak**: De Angular SPA draait op een andere origin dan Keycloak. Keycloak moet de juiste Web Origins geconfigureerd hebben voor de Angular-client.
- **Token lifetime**: De M2M demo-tokens bij Duende zijn geconfigureerd met `AccessTokenLifetime = int.MaxValue`. Bij Keycloak geldt een standaard lifetime. Voor de demo is dit acceptabel maar moet gedocumenteerd worden.
- **BFF sessie-isolatie**: Het API-scoped token in de Nuxt BFF moet **nooit** naar de browser gestuurd worden. Foutieve implementaties die het token in de response meegeven zijn een beveiligingsrisico.
- **docker-compose netwerken**: De M2M web UI en Nuxt BFF moeten de Organisation Registry API bereiken via het Docker-netwerk (niet via `localhost`), maar Keycloak moet bereikbaar zijn voor zowel browser-redirects (extern, poort 8180) als server-side token requests (intern, via Docker-netwerk).

---

## Requirements *(mandatory)*

### Functionele Requirements

- **FR-001**: Keycloak 24.0 MOET een realm `wegwijs` bevatten met testgebruikers die dezelfde claims hebben als de bestaande Duende IdentityServer gebruikers (`iv_wegwijs_rol_3D`, `vo_id`, `family_name`, `given_name`).
- **FR-002**: Keycloak MOET een OIDC-client `organisation-registry-local-dev` configureren voor Authorization Code + PKCE flow, met `redirect_uri` `https://organisatie.dev-vlaanderen.local/#/oic` en `https://organisatie.dev-vlaanderen.local/v2/oic`.
- **FR-003**: Keycloak MOET een client `cjmClient` configureren voor Client Credentials flow met scope `dv_organisatieregister_cjmbeheerder`.
- **FR-004**: Keycloak MOET een introspection endpoint blootstellen dat de Organisation Registry API kan gebruiken om M2M-tokens te valideren.
- **FR-005**: De Organisation Registry API MOET configureerbaar zijn om Keycloak als authority te gebruiken via `appsettings.keycloak.json` (of vergelijkbare override), zonder wijzigingen aan de Angular broncode.
- **FR-006**: De M2M web UI MOET een Docker-container zijn die bereikbaar is via de browser, met drie knoppen: "Authenticate", "Allowed call", "Forbidden call".
- **FR-007**: De M2M web UI MOET de HTTP-statuscode en relevante response-informatie tonen na elke API-aanroep.
- **FR-008**: De Nuxt 3 BFF MOET de authorization code flow volledig server-side afhandelen. Het access token voor de Organisation Registry API mag nooit in de browser-response terechtkomen.
- **FR-009**: De Nuxt 3 BFF MOET twee API-aanroepen uitvoeren na login: één naar een endpoint waarvoor de gebruiker rechten heeft, één naar een endpoint waarvoor niet — en beide resultaten tonen.
- **FR-010**: De Keycloak realm-configuratie MOET als JSON export-bestand in `./keycloak/` staan zodat `docker-compose up` automatisch de realm importeert.
- **FR-011**: `docker-compose up` MOET alle services starten (Keycloak, M2M web UI, Nuxt BFF) zonder handmatige tussenkomst.

### Key Entities *(include if feature involves data)*

- **Keycloak Realm `wegwijs`**: De Keycloak-realm met clients, gebruikers, scopes en claim-mappers die de bestaande Duende-configuratie spiegelt.
- **Keycloak Client `organisation-registry-local-dev`**: OIDC-client voor de Angular SPA, Authorization Code + PKCE, met Web Origins voor CORS.
- **Keycloak Client `cjmClient`**: Confidential client voor Client Credentials, met scope `dv_organisatieregister_cjmbeheerder`.
- **Keycloak Client `organisation-registry-api`**: Resource server client voor introspection, met secret `a_very=Secr3t*Key`.
- **M2M Demo Web UI**: .NET 8 Minimal API of ASP.NET Core Razor Pages, Docker-container, HTML-pagina met drie knoppen en resultatenweergave.
- **Nuxt 3 BFF**: Nuxt 3 server-side rendered applicatie met `@sidebase/nuxt-auth` of eigen server middleware voor OIDC session management, Docker-container.
- **appsettings.keycloak.json**: Configuratie-override voor de Organisation Registry API die Keycloak als authority configureert.

---

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: `docker-compose up` start alle services zonder fouten en Keycloak is beschikbaar op `http://localhost:8180` binnen 60 seconden.
- **SC-002**: Een ontwikkelaar kan de Angular SPA opstarten, inloggen via Keycloak met `dev`/`dev`, en een custom JWT ontvangen — aantoonbaar via de browser developer tools (Network tab).
- **SC-003**: De M2M web UI toont na klikken op "Authenticate" een geldig access token van Keycloak. Na "Allowed call" toont de pagina een 2xx-statuscode. Na "Forbidden call" toont de pagina exact `403 Forbidden`.
- **SC-004**: De Nuxt 3 BFF toont na login twee API-resultaten: één succesvol (2xx) en één verboden (403), waarbij het access token **niet** zichtbaar is in de browser developer tools (Network tab).
- **SC-005**: De Keycloak realm-configuratie is volledig gedefinieerd in `./keycloak/realm-export.json` en wordt automatisch geïmporteerd bij `docker-compose up keycloak`.
- **SC-006**: De drie demo-componenten zijn onafhankelijk te starten en te testen (elk kan apart `up` worden gezet mits Keycloak en de API draaien).
