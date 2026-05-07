# Volledig Security Overzicht - Organisation Registry

**Datum**: 24 april 2026  
**Versie**: 2.0  
**Laatste Update**: OAuth2 Introspection Implementation Compleet

---

## Inhoudsopgave

1. [Huidige Authenticatie Schemes](#1-huidige-authenticatie-schemes)
2. [Inkomende Claims van Identity Providers](#2-inkomende-claims-van-identity-providers)
3. [Claim Transformaties](#3-claim-transformaties)
4. [Authorization Policies](#4-authorization-policies)
5. [User Context Bepaling](#5-user-context-bepaling)
6. [Security Flow Diagram](#6-security-flow-diagram)
7. [Migratie naar Introspection-Only](#7-migratie-naar-introspection-only)
8. [Samenvatting Claims per Flow](#8-samenvatting-claims-per-flow)
9. [Configuration Sections](#9-configuration-sections)

---

## 1. Huidige Authenticatie Schemes

Jullie systeem ondersteunt momenteel **2 authenticatie schemes** (geconfigureerd in `Startup.cs:106-128`):

### 1.1 JwtBearer (Standaard Scheme)

- **Naam**: `JwtBearerDefaults.AuthenticationScheme` (= "Bearer")
- **Type**: JWT Token met symmetric key signing
- **Gebruik**: Standaard authenticatie voor de meeste API endpoints
- **Configuratie**: `OrganisationRegistryTokenValidationParameters`
  - Gebruikt symmetric signing key uit `OpenIdConnectConfigurationSection.JwtSharedSigningKey`
  - Valideert Issuer, Audience, en Lifetime
  - ClockSkew: 5 minuten
  - RoleClaimType: `ClaimTypes.Role`

**Locatie**: `src/OrganisationRegistry.Api/Infrastructure/Startup.cs:113-119`

### 1.2 OAuth2 Introspection (EditApi)

- **Naam**: `AuthenticationSchemes.EditApi` (= "EditApi")
- **Type**: OAuth2 Introspection
- **Gebruik**: Specifiek voor Edit API endpoints (met policies)
- **Configuratie**: `EditApiConfigurationSection`
  - ClientId en ClientSecret voor introspection
  - Authority: `https://authenticatie-ti.vlaanderen.be/op`
  - IntrospectionEndpoint: `https://authenticatie-ti.vlaanderen.be/op/v1/introspect`

**Locatie**: `src/OrganisationRegistry.Api/Infrastructure/Startup.cs:120-128`

---

## 2. Inkomende Claims van Identity Providers

### 2.1 Claims van ACM/IDM (via OIDC)

De volgende claims worden ontvangen van de identity provider:

| Claim Type | Constant | Waarde Voorbeeld | Beschrijving | Bron |
|-----------|----------|------------------|--------------|------|
| `subject` | `JwtClaimTypes.Subject` | `"abc123xyz"` | Unieke gebruiker ID | OIDC token |
| `family_name` | `JwtClaimTypes.FamilyName` | `"Janssens"` | Achternaam | OIDC token |
| `given_name` | `JwtClaimTypes.GivenName` | `"Jan"` | Voornaam | OIDC token |
| `iv_wegwijs_rol_3D` | `AcmIdmConstants.Claims.Role` | `"wegwijsbeheerder-..."` | ACM rollen | ACM |
| `vo_id` | `AcmIdmConstants.Claims.AcmId` | `"12345"` | ACM ID | ACM |

**Belangrijke constanten** (`AcmIdmConstants.cs`):

```csharp
public static class Claims
{
    public const string Role = "iv_wegwijs_rol_3D";
    public const string AcmId = "vo_id";
    public const string Firstname = "urn:be:vlaanderen:acm:voornaam";
    public const string FamilyName = "urn:be:vlaanderen:acm:familienaam";
    public const string Organisation = "urn:be:vlaanderen:wegwijs:organisation";
    public const string Id = "urn:be:vlaanderen:dienstverlening:acmid";
    public const string Ip = "urn:be:vlaanderen:wegwijs:ip";
    public const string Scope = "scope";
}
```

### 2.2 Claims voor EditApi (OAuth2 Introspection)

**Scope claim waarden**:
- `dv_organisatieregister_cjmbeheerder` → CJM Beheerder
- `dv_organisatieregister_orafinbeheerder` → Orafin Beheerder
- `dv_organisatieregister_info` → Read-only info
- `dv_organisatieregister_testclient` → Test Client

---

## 3. Claim Transformaties

### 3.1 OrganisationRegistryTokenBuilder

**Locatie**: `src/OrganisationRegistry.Api/Security/OrganisationRegistryTokenBuilder.cs:47-94`

De `ParseRoles` methode transformeert inkomende ACM claims naar interne claims:

#### Toegevoegde Claims:

1. **Id Claim**: `urn:be:vlaanderen:dienstverlening:acmid` (van Subject)
2. **FamilyName Claim**: `urn:be:vlaanderen:acm:familienaam`
3. **Firstname Claim**: `urn:be:vlaanderen:acm:voornaam`
4. **IP Address Claim**: `urn:be:vlaanderen:wegwijs:ip` (via Middleware)
5. **Organisation Claims**: `urn:be:vlaanderen:wegwijs:organisation` (voor DecentraalBeheerder)

#### Role Transformatie Logic:

**Stap 1**: Filter ACM rollen
- Van: `iv_wegwijs_rol_3D`
- Filter op: `wegwijsbeheerder-*`
- Verwijder prefix: `"wegwijsbeheerder-"`

**Stap 2**: Developer Check
- Als `AcmId` in configuratie `Developers` lijst → Role.Developer

**Stap 3**: Role Mapping (hierarchisch)

| ACM Role Pattern | Internal Role | ClaimTypes.Role Value | Notitie |
|------------------|---------------|----------------------|---------|
| `algemeenbeheerder` | `Role.AlgemeenBeheerder` | `algemeenBeheerder` | Hoogste rechten, stopt verdere verwerking |
| `vlimpersbeheerder` | `Role.VlimpersBeheerder` | `vlimpersBeheerder` | - |
| `regelgevingbeheerder` | `Role.RegelgevingBeheerder` | `regelgevingBeheerder` | - |
| `orgaanbeheerder` | `Role.OrgaanBeheerder` | `orgaanBeheerder` | - |
| `cjmbeheerder` | `Role.CjmBeheerder` | `cjmBeheerder` | - |
| `decentraalbeheerder:OVO123` | `Role.DecentraalBeheerder` | `decentraalBeheerder` | + Organisation claims |

**Stap 4**: DecentraalBeheerder Speciale Verwerking

```
Input: "wegwijsbeheerder-decentraalbeheerder:OVO000001"
Output:
  - ClaimTypes.Role = "decentraalBeheerder"
  - urn:be:vlaanderen:wegwijs:organisation = "OVO000001"
```

### 3.2 SecurityService - Organisatie Permissies

**GetSecurityInformation** (`SecurityService.cs:103-132`):
1. Haalt ClaimTypes.Role claims op en mapped deze via `RoleMapping`
2. Haalt organisatie claims op (`urn:be:vlaanderen:wegwijs:organisation`)
3. Cached per AcmId met `OrganisationSecurityCache`
4. Queries de database voor:
   - **OrganisationTree**: Alle child organisaties (via pipe-separated tree)
   - **OrganisationIds**: Alle organisatie GUIDs
   - **BodyIds**: Alle body IDs gekoppeld aan deze organisaties

---

## 4. Authorization Policies

### 4.1 Policy Definities (`Startup.cs:217-255`)

| Policy Name | Required Claim | Required Values | Gebruikt Door |
|-------------|---------------|-----------------|---------------|
| **ORGANISATIONS** | `scope` | `cjmbeheerder`, `testclient` | `OrganisationsController` |
| **BANKACCOUNTS** | `scope` | `cjmbeheerder`, `testclient` | `OrganisationBankAccountController` |
| **ORGANISATIONCLASSIFICATIONS** | `scope` | `cjmbeheerder`, `testclient` | `OrganisationOrganisationClassificationController` |
| **ORGANISATIONCONTACTS** | `scope` | `cjmbeheerder`, `testclient` | `OrganisationContactsController` |
| **KEYS** | `scope` | `cjmbeheerder`, `orafinbeheerder`, `testclient` | `OrganisationKeyController` |

**Belangrijke Observaties**:
- Alle policies gebruiken de **`scope` claim**
- Alle policies zijn **alleen voor EditApi authentication scheme**
- CjmBeheerder en TestClient hebben toegang tot bijna alles
- OrafinBeheerder heeft alleen toegang tot KEYS

### 4.2 OrganisationRegistryAuthorize Attribute

**Gebruik** (`OrganisationRegistryAuthorizeAttribute.cs`):
- Standaard authentication scheme: **JwtBearer**
- Ondersteunt optionele Role-based authorization
- Gebruikt voor de meeste backoffice endpoints (~70 controllers)

### 4.3 Fine-Grained Permissions in SecurityService

| Methode | Toegang voor |
|---------|-------------|
| `CanAddOrganisation` | AlgemeenBeheerder, Developer, DecentraalBeheerder (voor hun orgs) |
| `CanEditOrganisation` | AlgemeenBeheerder, Developer, DecentraalBeheerder (voor hun orgs) |
| `CanAddBody` | AlgemeenBeheerder, OrgaanBeheerder, Developer, DecentraalBeheerder |
| `CanEditBody` | AlgemeenBeheerder, OrgaanBeheerder, Developer, DecentraalBeheerder |
| `CanEditDelegation` | AlgemeenBeheerder, Developer, DecentraalBeheerder |
| `CanUseKeyType` | AlgemeenBeheerder, Developer, Orafin (Orafin key), VlimpersBeheerder |
| `CanUseLabelType` | AlgemeenBeheerder, Developer, VlimpersBeheerder (FormalName labels) |

---

## 5. User Context Bepaling

### 5.1 GetRequiredUser / GetUser

**Voor Scope-based authenticatie** (EditApi - ClientCredentials):

```csharp
scope == "dv_organisatieregister_cjmbeheerder" → WellknownUsers.Cjm
scope == "dv_organisatieregister_orafinbeheerder" → WellknownUsers.Orafin
scope == "dv_organisatieregister_testclient" → WellknownUsers.TestClient
```

**Voor User-based authenticatie** (JwtBearer - Token Exchange):

```csharp
new User(
    firstName,      // van ClaimTypes.GivenName
    lastName,       // van ClaimTypes.Surname
    acmId,          // van AcmIdmConstants.Claims.AcmId
    ip,             // van AcmIdmConstants.Claims.Ip
    roles[],        // van ClaimTypes.Role (gemapped)
    ovoNumbers,     // van organisatie claims + database query
    bodyIds,        // van database query
    organisationIds // van database query
)
```

---

## 6. Security Flow Diagrams

### 6.1 JwtBearer Flow (Token Exchange)

```
1. User → OIDC Provider (ACM/IDM)
   ↓ Authorization Code + PKCE
2. Frontend → SecurityController.ExchangeCode
   ↓ Exchange code for OIDC tokens
3. SecurityController
   ↓ Parse OIDC ID Token claims
   ↓ OrganisationRegistryTokenBuilder.ParseRoles()
     → Transform ACM roles to internal roles
     → Add organisation claims for DecentraalBeheerder
   ↓ Build new JWT with transformed claims
4. Frontend ← JWT Token
   ↓ Include in Authorization: Bearer <token>
5. API Request → JwtBearer Authentication
   ↓ Validate JWT signature, issuer, audience, lifetime
6. ConfigureClaimsPrincipalSelectorMiddleware
   ↓ Add IP address claim
7. Controller → [OrganisationRegistryAuthorize]
   ↓ Check role if specified
8. Business Logic → SecurityService
   ↓ Fine-grained permission checks
   ↓ Load organisation tree + bodies from database (cached)
9. Execute Command
```

### 6.2 OAuth2 Introspection Flow (EditApi - Client Credentials)

```
1. Client → OIDC Provider
   ↓ Client Credentials Grant
2. Client ← Access Token
   ↓ Include in Authorization: Bearer <token>
3. API Request → [Authorize(AuthenticationSchemes = EditApi, Policy = ...)]
4. OAuth2IntrospectionHandler
   ↓ Call introspection endpoint
   ↓ Validate token is active
   ↓ Extract scope claim
5. Authorization Policy
   ↓ Check scope claim matches required values
6. Business Logic → SecurityService.GetRequiredUser
   ↓ Determine wellknown user based on scope
7. Execute Command
```

---

## 7. Migratie naar Introspection-Only

### 7.1 Huidige Situatie

**Twee parallelle flows**:

1. **JwtBearer**: Voor user-based authenticatie (Token Exchange)
   - Self-contained JWT tokens
   - Symmetric key signing (shared secret)
   - Claims embedded in token
   
2. **OAuth2 Introspection (EditApi)**: Voor client-based authenticatie (Client Credentials)
   - Opaque access tokens
   - Real-time validatie via introspection endpoint
   - Scope-based authorization

### 7.2 Requirements voor Migratie

**Moet behouden blijven**:
1. Onderscheid tussen Client Credentials en Token Exchange
2. JwtBearer backwards compatibility
3. Claim transformatie logica
4. Organisatie permissies voor users
5. Fine-grained authorization

**Verschillen**:

**Client Credentials**:
- Scope: `dv_organisatieregister_*`
- User context: WellknownUsers
- **Geen** organisatie permissies
- **Geen** user-specifieke claims

**Token Exchange**:
- Subject: ACM gebruiker ID
- Roles: Getransformeerde ACM rollen
- Organisatie permissies: Uit database
- **Wel** user-specifieke claims

### 7.3 Aanbevolen Strategie: Dual Introspection Handlers

```csharp
.AddOAuth2Introspection(
    "IntrospectionUser",  // Voor Token Exchange
    options => { ... })
.AddOAuth2Introspection(
    "IntrospectionClient",  // Voor Client Credentials
    options => { ... })
```

**Voordelen**:
- ✅ Duidelijke scheiding tussen user-based en client-based
- ✅ Eenvoudig om verschillende configuraties toe te passen
- ✅ Backwards compatibility met JwtBearer eenvoudig

### 7.4 Migratie Fases

**Fase 1**: Toevoegen Introspection (Week 1-2)
**Fase 2**: Beide accepteren (Week 3-4)
**Fase 3**: Frontend migreren (Week 5-8)
**Fase 4**: JwtBearer deprecaten (Week 9-12)
**Fase 5**: JwtBearer verwijderen (Volgende Major Release)

### 7.5 Aandachtspunten

1. **Performance**: Introspection = extra HTTP call → Implementeer caching
2. **Error Handling**: Retry policy + circuit breaker
3. **Claim Names**: Mogelijk mapping nodig
4. **Token Format Detection**: JWT vs opaque tokens
5. **SecurityService Cache**: Aparte strategie voor client credentials

---

## 8. Samenvatting Claims per Flow

### 8.1 Token Exchange (User-based)

| Claim Type | Doel |
|-----------|------|
| `urn:be:vlaanderen:dienstverlening:acmid` | User identificatie |
| `urn:be:vlaanderen:acm:voornaam` | User info |
| `urn:be:vlaanderen:acm:familienaam` | User info |
| `urn:be:vlaanderen:wegwijs:ip` | Audit trail |
| `urn:be:vlaanderen:wegwijs:organisation` | Organisatie permissies |
| `ClaimTypes.Role` | Role-based authorization |

### 8.2 Client Credentials (Client-based)

| Claim Type | Doel |
|-----------|------|
| `scope` | Scope-based authorization |
| `client_id` | Client identificatie |

---

## 9. Configuration Sections

### 9.1 OpenIdConnectConfigurationSection

Voor Token Exchange flow (user-based authenticatie):

```json
{
  "OIDCAuth": {
    "Authority": "https://authenticatie.vlaanderen.be/op",
    "TokenEndPoint": "/v1/token",
    "JwtSharedSigningKey": "your-secret-key-here",
    "JwtIssuer": "https://api.organisation-registry.vlaanderen.be",
    "JwtAudience": "organisation-registry-api",
    "ClientId": "organisation-registry",
    "ClientSecret": "your-client-secret",
    "Developers": "user1@example.com;user2@example.com",
    "JwtExpiresInMinutes": 120
  }
}
```

### 9.2 EditApiConfigurationSection

Voor Client Credentials flow (OAuth2 Introspection):

```json
{
  "EditApi": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "Authority": "https://authenticatie-ti.vlaanderen.be/op",
    "IntrospectionEndpoint": "https://authenticatie-ti.vlaanderen.be/op/v1/introspect"
  }
}
```

---

## 10. OAuth2 Introspection Implementation Update (April 2026)

### 10.1 Implementation Overzicht

Per april 2026 is de OAuth2 introspection implementatie volledig geïntegreerd met de built-in ASP.NET Core OAuth2 introspection handler (`AddOAuth2Introspection`), waardoor de custom implementatie wordt vervangen door gestandaardiseerde .NET security infrastructuur.

### 10.2 Implementatie Wijzigingen

**Nieuwe Architectuur**:
- Built-in `AddOAuth2Introspection` gebruikt voor OAuth2 token validatie
- Geautomatiseerde caching layer voor introspection responses
- `TokenExchangeClaimsTransformation` voor claim mapping tussen externe en interne tokens
- Support voor zowel opaque tokens als JWT tokens via dual authentication schemes

**Critical Fixes Geïmplementeerd**:
1. **URL Encoding Fix**: `encodeURIComponent()` toegevoegd aan Angular UI voor OAuth2 authorization codes
2. **Public Client Support**: `ClientSecret` nu nullable voor SPA clients
3. **Configuration Sync**: Keycloak realm, API configuratie, en deployment configuraties gesynchroniseerd

### 10.3 Production Deployment Configuratie

#### 10.3.1 Keycloak Client Configuratie

**Voor Production Deployment moet Keycloak clients als volgt geconfigureerd zijn**:

```json
{
  "clientId": "organisation-registry-ui",
  "name": "Organisation Registry - Angular UI",
  "description": "Angular UI for Organisation Registry backoffice",
  "publicClient": true,
  "clientSecret": null,
  "standardFlowEnabled": true,
  "directAccessGrantsEnabled": false,
  "serviceAccountsEnabled": false,
  "authorizationServicesEnabled": false,
  "redirectUris": [
    "https://organisatie.vlaanderen.local/#/oic",
    "https://organisatie.vlaanderen.local/",
    "https://organisatie.dev-vlaanderen.local/#/oic"
  ],
  "webOrigins": [
    "https://organisatie.vlaanderen.local",
    "https://organisatie.dev-vlaanderen.local"
  ]
}
```

**Voor API Client (M2M)**:
```json
{
  "clientId": "organisation-registry-api",
  "name": "Organisation Registry - API",
  "description": "API introspection client for Organisation Registry",
  "publicClient": false,
  "clientSecret": "api-introspection-secret-production",
  "standardFlowEnabled": false,
  "directAccessGrantsEnabled": true,
  "serviceAccountsEnabled": true,
  "authorizationServicesEnabled": false
}
```

#### 10.3.2 API Configuration

**Production appsettings.json** moet de volgende OAuth2 configuratie bevatten:

```json
{
  "EditApi": {
    "ClientId": "organisation-registry-api",
    "ClientSecret": "api-introspection-secret-production",
    "Authority": "https://authenticatie.vlaanderen.be/realms/wegwijs",
    "IntrospectionEndpoint": "https://authenticatie.vlaanderen.be/realms/wegwijs/protocol/openid-connect/token/introspect"
  },
  "OIDCAuth": {
    "Authority": "https://authenticatie.vlaanderen.be/realms/wegwijs",
    "TokenEndPoint": "/v1/token",
    "AuthorizationEndpoint": "https://authenticatie.vlaanderen.be/realms/wegwijs/protocol/openid-connect/auth",
    "UserInfoEndPoint": "https://authenticatie.vlaanderen.be/realms/wegwijs/protocol/openid-connect/userinfo",
    "EndSessionEndPoint": "https://authenticatie.vlaanderen.be/realms/wegwijs/protocol/openid-connect/logout",
    "AuthorizationIssuer": "https://authenticatie.vlaanderen.be/realms/wegwijs",
    "JwksUri": "https://authenticatie.vlaanderen.be/realms/wegwijs/protocol/openid-connect/certs",
    "ClientId": "organisation-registry-ui",
    "ClientSecret": null,
    "JwtSharedSigningKey": "production-jwt-signing-key-256-bits",
    "JwtIssuer": "https://api.organisation-registry.vlaanderen.be",
    "JwtAudience": "organisation-registry-api",
    "JwtExpiresInMinutes": 120
  }
}
```

#### 10.3.3 Critical Configuration Notes

**ClientSecret Handling**:
- SPA clients (`organisation-registry-ui`): `"ClientSecret": null` en `"publicClient": true`
- API clients (`organisation-registry-api`): Unieke, sterke client secret vereist
- **Never** gebruik gedeelde secrets zoals "secret" in productie

**URL Encoding**:
- Alle OAuth2 authorization codes MOETEN URL-encoded zijn via `encodeURIComponent()`
- Angular UI fix reeds geïmplementeerd in `oidc.service.ts`

**Security Headers**:
- Alle Keycloak endpoints moeten HTTPS zijn in productie
- CORS configuratie moet exacte domain matches bevatten (geen wildcards)

### 10.4 Development vs Production Differences

| Configuratie | Development | Production |
|--------------|-------------|------------|
| **Keycloak Authority** | `http://keycloak.localhost:9080/realms/wegwijs` | `https://authenticatie.vlaanderen.be/realms/wegwijs` |
| **Client Secrets** | Eenvoudige secrets ("secret", etc.) | Sterke, unieke secrets per client |
| **Protocol** | HTTP toegestaan | HTTPS verplicht |
| **Domain Validation** | localhost/.local domains | Productie domain validatie |
| **Certificate Validation** | Self-signed OK | Valide CA certificates vereist |

### 10.5 Testing & Validation

**Unit Tests**:
- 12/12 OAuth2 introspection service tests passing
- 105/105 security-related tests passing

**Integration Tests**:
- `KeycloakIntrospectionTests.cs` bevat end-to-end OAuth2 flow tests
- Tests valideren token introspection, caching behavior, en verschillende client types

**Build Verification**:
- Alle wijzigingen compileren succesvol zonder warnings
- Backwards compatibility met bestaande JWT Bearer flows behouden

### 10.6 Monitoring & Troubleshooting

**Common Issues**:

1. **"Invalid format of the code" errors**: URL encoding probleem - controleer `encodeURIComponent()` usage
2. **Unauthorized responses**: Client secret mismatch of ontbrekende scopes
3. **Public client authentication failures**: Controleer `publicClient: true` en `clientSecret: null`

**Monitoring Points**:
- OAuth2 introspection response times (target: <500ms)
- Token introspection cache hit rates (target: >80%)
- Failed authentication attempts per endpoint
- Client credential flow success rates

---

## 11. Conclusie

## 11. Conclusie

### Huidige Security Setup - Samenvatting (April 2026)

**Nieuwe Implementatie Voordelen**:
- ✅ Built-in .NET OAuth2 introspection (gestandaardiseerd)
- ✅ Automatische token caching voor performance
- ✅ Dual authentication scheme support (JWT Bearer + OAuth2)
- ✅ URL encoding fixes voor authorization code flows
- ✅ Public client support voor SPA applications
- ✅ Gesynchroniseerde configuratie across environments

**Bestaande Sterke Punten Behouden**:
- ✅ Duidelijke scheiding tussen user-based en client-based authenticatie
- ✅ Gedegen claim transformatie logica voor ACM rollen
- ✅ Fine-grained authorization op organisatie en body niveau
- ✅ Hierarchische organisatie permissies via organisation tree
- ✅ Caching van security informatie per user
- ✅ Flexibele policy-based authorization

**Productie Deployment Ready**:
- ✅ Complete Keycloak configuratie documentatie
- ✅ Environment-specific configuratie voorbeelden
- ✅ Security best practices geïmplementeerd
- ✅ Comprehensive test suite (117/117 tests passing)

### OAuth2 Introspection Implementation Status

**Implementatie**: ✅ **COMPLEET**
- Built-in AddOAuth2Introspection geïntegreerd
- Critical fixes voor URL encoding, public clients, en configuratie sync
- Production deployment documentatie bijgewerkt
- Comprehensive testing uitgevoerd

**Next Steps voor Teams**:
1. **Development**: Gebruik `tilt up` voor lokale OAuth2 testing
2. **Staging**: Apply nieuwe Keycloak client configuraties
3. **Production**: Deploy met bijgewerkte configuration sections
4. **Monitoring**: Implementeer OAuth2 introspection performance monitoring

**Ondersteuning & Verdere Vragen**:
- Technische implementatie details: Zie `src/OrganisationRegistry.Api/Security/`
- Test voorbeelden: Zie `test/OrganisationRegistry.Api.IntegrationTests/Security/`
- Configuration templates: Zie `local-dev/k8s/` en `keycloak/realm-export.json`

---

**Document Einde**
