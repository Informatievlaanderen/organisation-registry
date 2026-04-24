# Checklist: 005 — Policy-based authorization refactoring

## Requirements

### R1 — BackofficeUser policy toegevoegd
- [ ] `PolicyNames.BackofficeUser` constante toegevoegd in `PolicyNames.cs`
- [ ] Policy geregistreerd in `Startup.cs`: `RequireClaim(AcmIdmConstants.Claims.VoId)`
- [ ] Policy werkt voor zowel JwtBearer- als BffApi-scheme tokens

### R2 — Backoffice-controllers gebruiken policy-based auth
- [ ] `OrganisationRegistryAuthorizeAttribute` vermeldt geen schemes meer (of is deprecated)
- [ ] Alle Backoffice-controllers gebruiken `[Authorize(Policy = PolicyNames.BackofficeUser)]` voor basis-toegang
- [ ] Role-based checks (`[Authorize(Roles = ...)]`) blijven functioneel

### R3 — EditApi-controllers ongewijzigd
- [ ] Alle EditApi-controllers blijven `[Authorize(AuthenticationSchemes = EditApi, Policy = PolicyNames.X)]` gebruiken
- [ ] Geen regressies in EditApi-autorisatie

### R4 — BffApi feature-flag compatibiliteit
- [ ] Policy werkt correct als `FeatureManagement:BffApi = false`
- [ ] Policy werkt correct als `FeatureManagement:BffApi = true`

### R5 — Tests
- [ ] Unit/integratietests bijgewerkt voor policy-based auth
- [ ] Bestaande tests slagen nog steeds (`dotnet test`)

### R6 — Geen breaking changes in externe API
- [ ] HTTP status codes voor ongeauthenticeerde requests ongewijzigd (401)
- [ ] HTTP status codes voor ongeautoriseerde requests ongewijzigd (403)
