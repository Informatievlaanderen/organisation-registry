# 005 — Policy-based authorization refactoring

## Doel

De huidige autorisatie in de Backoffice API is scheme-gebaseerd: `OrganisationRegistryAuthorizeAttribute` vermeldt expliciet twee authentication schemes (`Bearer, Introspection`). Dit is een anti-patroon: het koppelt transport-mechanismen (hoe een token binnenkomt) aan toegangsbeheer (wat een token mag).

Het doel van deze feature is om de autorisatie te vervangen door **policy-based authorization** op basis van **OAuth2 scopes en claims**. Dit sluit aan op de bestaande aanpak in de EditApi-controllers (die al `PolicyNames` gebruiken) en maakt het mogelijk om in de toekomst het JwtBearer-scheme (self-minted HS256) te verwijderen.

---

## Achtergrond

### Huidige situatie

**Drie authentication schemes:**

| Scheme | Constante | Gebruikt door | Token-type |
|---|---|---|---|
| `Bearer` (JwtBearer) | `AuthenticationSchemes.JwtBearer` | Angular SPA, seed | Symmetric HS256 JWT (self-minted door `/v1/security/exchange`) |
| `Introspection` | `AuthenticationSchemes.Introspection` | M2M clients (Orafin, CJM) | OAuth2 access token (ACM/IDM introspection) |
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

**Gebruikerstoken (Angular SPA via `/v1/security/exchange`, of Nuxt BFF via Keycloak):**
- `vo_id` — uniek gebruikers-ID (aanwezig in ALLE gebruikerstokens, NIET in M2M-tokens)
- `given_name`, `family_name`
- `iv_wegwijs_rol_3D` → via `BffClaimsTransformation` omgezet naar `ClaimTypes.Role`

**M2M-token (client credentials via ACM/IDM):**
- `scope` — bevat o.a. `dv_organisatieregister_info` (in ALLE M2M-tokens)
- Specifieke scopes per client:
  - `dv_organisatieregister_orafinbeheerder`
  - `dv_organisatieregister_cjmbeheerder`
  - `dv_organisatieregister_testclient`
  - `dv_organisatieregister_powerBi`
- `dv_organisatieregister_orgcode` — OVO-code van de client (via introspection)

**Onderscheid:** `vo_id` aanwezig → gebruikerstoken. `dv_organisatieregister_info` in scope → M2M-token.

---

## Gewenste situatie

### 1. Backoffice-toegang via policy

Vervang `OrganisationRegistryAuthorizeAttribute` met `[Authorize(Policy = PolicyNames.BackofficeUser)]`.

Policy-definitie:
```csharp
options.AddPolicy(PolicyNames.BackofficeUser, builder =>
    builder.RequireClaim(AcmIdmConstants.Claims.VoId)); // "vo_id"
```

Dit zorgt dat enkel tokens met een `vo_id` (= authentieke gebruikerstokens) de Backoffice kunnen bereiken, ongeacht het authentication scheme.

### 2. Role-based checks blijven via claims

De huidige `[OrganisationRegistryAuthorize(Role.AlgemeenBeheerder)]`-stijl kan vervangen worden door `[Authorize(Roles = "algemeenbeheerder")]` of door een aparte policy per rol. De bestaande `BffClaimsTransformation` en `SecurityService` logica hoeft in eerste instantie niet te veranderen.

### 3. Schemes worden passief

Schemes authenticeren (verifiëren de token-handtekening en introspection), maar beslissen niet over toegang. Alle toegangsbeslissingen verlopen via policies.

### 4. BffApi-scheme integratie

Het `BffApi`-scheme (Keycloak introspection) produceert na `BffClaimsTransformation` dezelfde claims als het JwtBearer-scheme. De `BackofficeUser`-policy werkt voor beide.

### 5. Verwijdering van legacy JwtBearer (latere fase)

Het HS256 self-minted token-mechanisme (`/v1/security/exchange`) is een technische schuld. Zodra de Angular SPA overschakelt op directe Keycloak-authenticatie, kan dit scheme verwijderd worden. Dit valt buiten scope van deze feature, maar de refactoring maakt die stap eenvoudiger.

---

## Migratiepad

1. **Voeg `BackofficeUser`-policy toe** in `ConfigureEditApiAuthPolicies` (of een nieuwe methode)
2. **Vervang `OrganisationRegistryAuthorizeAttribute`** door `[Authorize(Policy = PolicyNames.BackofficeUser)]` op alle Backoffice-controllers
3. **Verwijder scheme-vermelding** uit `OrganisationRegistryAuthorizeAttribute` of deprecated het attribuut
4. **Behoud alle bestaande EditApi-policies** ongewijzigd
5. **Tests bijwerken** zodat ze policy-based autorisatie testen

---

## Risico's en aandachtspunten

- **`BffApi`-scheme is feature-flag afhankelijk** (`FeatureManagement:BffApi`). Zorg dat de policy ook werkt wanneer dit scheme uitgeschakeld is.
- **`OrganisationRegistryTokenValidationParameters`** (HS256 JwtBearer) moet blijven werken zolang de Angular SPA die tokens gebruikt.
- **`ConfigureClaimsPrincipalSelectorMiddleware`** selecteert welke `ClaimsPrincipal` actief is bij meerdere geverifieerde schemes — dit gedrag verandert niet door deze refactoring.
- **Rolautorisatie via `[Authorize(Roles = ...)]`** werkt enkel als de juiste `RoleClaimType` ingesteld is per scheme. `BffClaimsTransformation` zet rollen als `ClaimTypes.Role` (lange URI). Dit is al gecorrigeerd in de Startup via `options.RoleClaimType = ClaimTypes.Role`.

---

## Niet in scope

- Verwijdering van het HS256 JwtBearer-scheme
- Vervanging van `SecurityService` (die directe claim-checks doet)
- Wijzigingen aan de EditApi-controllers (die al policy-based werken)
- Migratie van de Angular SPA naar Keycloak-tokens
