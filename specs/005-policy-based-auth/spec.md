# 005 — Policy-based authorization refactoring

## Doel

De huidige autorisatie in de Backoffice API is scheme-gebaseerd:
`OrganisationRegistryAuthorizeAttribute` vermeldt expliciet twee authentication schemes
(`Bearer, Introspection`). Dit is een anti-patroon: het koppelt transport-mechanismen
(hoe een token binnenkomt) aan toegangsbeheer (wat een token mag).

Het doel van deze feature is om de autorisatie te vervangen door **policy-based
authorization** op basis van **OAuth2 scopes en claims**, consistent met de aanpak die
al gebruikt wordt in de EditApi-controllers.

---

## Achtergrond

### Huidige situatie

**Drie authentication schemes:**

| Scheme | Constante | Gebruikt door | Token-type |
|---|---|---|---|
| `Bearer` (JwtBearer) | `AuthenticationSchemes.JwtBearer` | Angular SPA, seed | Symmetric HS256 JWT (self-minted door `/v1/security/exchange`) |
| `Introspection` | `AuthenticationSchemes.Introspection` | M2M clients (Orafin, CJM, ...) | OAuth2 access token (ACM/IDM introspection) |
| `BffApi` | `AuthenticationSchemes.BffApi` | Nuxt BFF | OAuth2 access token (Keycloak introspection) |

**`OrganisationRegistryAuthorizeAttribute`** (Backoffice controllers):
```csharp
AuthenticationSchemes = $"{Schemes.JwtBearer}, {Schemes.Introspection}";
```
Toegang is gebaseerd op scheme, niet op claims.

**EditApi-controllers** gebruiken al de gewenste aanpak:
```csharp
[Authorize(AuthenticationSchemes = EditApi, Policy = PolicyNames.Keys)]
```

### Claims per token-type

**Gebruikerstoken** (Angular SPA via `/v1/security/exchange`, of Nuxt BFF via Keycloak):
- `vo_id` — uniek gebruikers-ID (**aanwezig in ALLE gebruikerstokens, NOOIT in M2M-tokens**)
- `given_name`, `family_name`
- `iv_wegwijs_rol_3D` → via `BffClaimsTransformation` omgezet naar `ClaimTypes.Role`

**M2M-token** (client credentials via ACM/IDM):
- `scope` — bevat de functionele scope(s) van de client (zie tabel hieronder)
- `dv_organisatieregister_orgcode` — OVO-code van de client (via introspection, alleen
  aanwezig als de client `dv_organisatieregister_info` in zijn scopes heeft)

### M2M clients en hun scopes (uit ACM/IDM onboarding-dossier)

| Client | Functionele scope | `dv_organisatieregister_info` |
|---|---|---|
| Orafin (DFB) | `dv_organisatieregister_orafinbeheerder` | ja |
| CJM Kiosk | `dv_organisatieregister_cjmbeheerder` | ja |
| PowerBI | `dv_organisatieregister_powerBi` | ja |
| Test Client (DV) | alle vier functionele scopes | ja |
| **Introspection client** (`organisation-registry-api`) | — | ja |

**Belangrijk:** `dv_organisatieregister_info` is de scope die de API zelf nodig heeft
om de `orgcode`-claim op te mogen halen via het ACM/IDM introspection endpoint. Deze
scope is aanwezig in zowel API-consumers als de introspection client zelf, en is
**ongeschikt als differentiator** voor toegangsbeheer.

De introspection client heeft **geen** van de vier functionele scopes. De bestaande
EditApi-policies werken daardoor al correct: ze eisen één van de functionele scopes
en sluiten de introspection client automatisch uit.

### Hoe de vier M2M clients van elkaar onderscheiden worden

Via `RequireClaim("scope", ...)` per policy, met de relevante functionele scopes als
toegelaten waarden. Elke client heeft enkel zijn eigen functionele scope (behalve de
TestClient die alle scopes heeft voor testdoeleinden):

```csharp
// Alleen CJM en TestClient
options.AddPolicy(PolicyNames.Organisations,
    builder => builder.RequireClaim(AcmIdmConstants.Claims.Scope,
        AcmIdmConstants.Scopes.CjmBeheerder,
        AcmIdmConstants.Scopes.TestClient));

// Orafin, CJM én TestClient
options.AddPolicy(PolicyNames.Keys,
    builder => builder.RequireClaim(AcmIdmConstants.Claims.Scope,
        AcmIdmConstants.Scopes.CjmBeheerder,
        AcmIdmConstants.Scopes.OrafinBeheerder,
        AcmIdmConstants.Scopes.TestClient));
```

---

## Gewenste situatie

### 1. BackofficeUser policy

Nieuwe policy op basis van `vo_id` — het enige claim dat aanwezig is in alle
gebruikerstokens en nooit in M2M-tokens:

```csharp
options.AddPolicy(PolicyNames.BackofficeUser,
    builder => builder.RequireClaim(AcmIdmConstants.Claims.AcmId)); // "vo_id"
```

Alle Backoffice-controllers gebruiken:
```csharp
[Authorize(Policy = PolicyNames.BackofficeUser)]
```

Dit werkt voor zowel het JwtBearer-scheme (Angular SPA) als het BffApi-scheme
(Nuxt BFF via Keycloak), zonder schemes expliciet te vermelden in het attribuut.

### 2. `OrganisationRegistryAuthorizeAttribute` vereenvoudigd of deprecated

Het attribuut vermeldt geen schemes meer. Ofwel:
- het attribuut wordt een dunne wrapper om `[Authorize(Policy = PolicyNames.BackofficeUser)]`
- of het attribuut wordt deprecated en controllers worden direct geannoteerd

Role-based verfijning (bijv. `AlgemeenBeheerder`) blijft via `[Authorize(Roles = ...)]`
of een aanvullende policy, bovenop de BackofficeUser-basischeck.

### 3. EditApi-policies ongewijzigd

De bestaande policies (`Keys`, `Organisations`, `BankAccounts`, `OrganisationClassifications`,
`OrganisationContacts`) en hun controllers blijven ongewijzigd. Ze werken al correct.

### 4. Schemes worden passief

Schemes authenticeren (token-handtekening + introspection), maar beslissen niet over
toegang. Alle toegangsbeslissingen verlopen via policies.

### 5. `dv_organisatieregister_powerBi` (toekomstig)

PowerBI heeft enkel `dv_organisatieregister_powerBi` + `info`. Er is nog geen
dedicated policy voor PowerBI in de codebase. Dit valt buiten scope van deze feature,
maar bij het toevoegen van een PowerBI-endpoint hoeft enkel een nieuwe policy
toegevoegd te worden:
```csharp
options.AddPolicy(PolicyNames.PowerBi,
    builder => builder.RequireClaim(AcmIdmConstants.Claims.Scope,
        AcmIdmConstants.Scopes.PowerBi,
        AcmIdmConstants.Scopes.TestClient));
```

---

## Migratiepad

1. Voeg `PolicyNames.BackofficeUser` toe aan `PolicyNames.cs`
2. Voeg `AcmIdmConstants.Scopes.PowerBi` toe aan `AcmIdmConstants.cs` (ontbreekt nog)
3. Registreer `BackofficeUser`-policy in `Startup.cs`
4. Vervang `[OrganisationRegistryAuthorize]` door `[Authorize(Policy = PolicyNames.BackofficeUser)]`
   op alle Backoffice-controllers
5. Verwijder scheme-vermelding uit `OrganisationRegistryAuthorizeAttribute` of markeer deprecated
6. Tests bijwerken

---

## Risico's en aandachtspunten

- **`BffApi`-scheme is feature-flag afhankelijk** (`FeatureManagement:BffApi`). De policy
  werkt ongeacht welk scheme het token heeft geverifieerd — dat is precies het voordeel.
- **`OrganisationRegistryTokenValidationParameters`** (HS256 JwtBearer) blijft nodig
  zolang de Angular SPA self-minted tokens gebruikt.
- **`ConfigureClaimsPrincipalSelectorMiddleware`** verandert niet.
- **Rolautorisatie** via `[Authorize(Roles = ...)]` werkt alleen als `RoleClaimType`
  correct is per scheme. Dit is al gecorrigeerd voor BffApi in Startup.cs.

---

## Niet in scope

- Verwijdering van het HS256 JwtBearer-scheme en `/v1/security/exchange`
- Vervanging van `SecurityService` (doet directe claim-checks)
- Migratie van de Angular SPA naar Keycloak-tokens
- Nieuwe PowerBI-endpoints
