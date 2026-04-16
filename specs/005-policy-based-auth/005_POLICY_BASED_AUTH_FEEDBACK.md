# 005 Policy-Based Auth - implementatie en feedback

## Uitgevoerde wijzigingen

- `PolicyNames.BackofficeUser` toegevoegd.
- `AcmIdmConstants.Scopes.PowerBi` toegevoegd voor een toekomstige PowerBI-policy, omdat dit expliciet in het migratiepad van de 005-specificatie stond.
- `BackofficeUser` policy centraal geregistreerd in `Startup`.
- `OrganisationRegistryAuthorizeAttribute` omgebouwd naar een wrapper rond `PolicyNames.BackofficeUser`.
- Scheme-vermelding verwijderd uit `OrganisationRegistryAuthorizeAttribute`.
- Bestaande role mapping in `OrganisationRegistryAuthorizeAttribute(params Role[])` behouden.
- `GetAuthenticateInfoAsync()` uitgebreid zodat directe checks na autorisatie zowel token-exchange/JWT (`Bearer`) als BFF (`BffApi`) kunnen gebruiken.
- Authorization contracttests uitgebreid voor policy-based Backoffice-auth.

## Ontwerpfeedback

### 1. Policy-only controllers zijn niet genoeg zonder centrale scheme-keuze

De oorspronkelijke richting was om controllers te annoteren met:

```csharp
[Authorize(Policy = PolicyNames.BackofficeUser)]
```

Met de huidige ASP.NET Core configuratie gebruikt de API echter `Bearer` als default authenticate scheme. Een BFF-token wordt daardoor niet automatisch via `BffApi` geauthenticeerd.

Aanbevolen en geimplementeerde oplossing: zet de toegestane schemes centraal op de `BackofficeUser` policy. Controllers blijven scheme-vrij, maar de policy weet dat Backoffice-gebruikers via `Bearer` en conditioneel via `BffApi` kunnen binnenkomen.

### 2. `BffApi` moet conditioneel in de policy

`BffApi` wordt alleen geregistreerd wanneer `FeatureManagement:BffApi` aan staat. Daarom mag de policy `BffApi` ook alleen toevoegen wanneer die feature flag aan staat. Anders kan authorization verwijzen naar een ongeregistreerd scheme.

Geimplementeerd gedrag:

- Feature flag uit: `BackofficeUser` gebruikt alleen `Bearer`.
- Feature flag aan: `BackofficeUser` gebruikt `Bearer` en `BffApi`.

### 3. `vo_id` is de juiste differentiator voor Backoffice

Backoffice-toegang is gebaseerd op `AcmIdmConstants.Claims.AcmId`, met waarde `vo_id`.

Dit sluit M2M-tokens uit, omdat die scopes hebben maar geen gebruikersclaim `vo_id`. De bestaande EditApi-policies blijven scope-based en blijven dus het juiste pad voor M2M-clients.

### 4. Waarom is `AcmIdmConstants.Scopes.PowerBi` toegevoegd?

De `PowerBi` constant is niet nodig om token-exchange/JWT of BFF-authenticatie voor Backoffice te laten werken. De `BackofficeUser` policy gebruikt alleen `vo_id` en kijkt niet naar PowerBI-scopes.

De reden voor de toevoeging is dat de 005-specificatie PowerBI beschrijft als toekomstige M2M-client met scope `dv_organisatieregister_powerBi`, en in het migratiepad expliciet vraagt om `AcmIdmConstants.Scopes.PowerBi` toe te voegen. Het is dus een voorbereidende constante voor een latere PowerBI-policy of endpoint, niet een vereiste voor de huidige Backoffice-auth wijziging.

Als we de wijziging strikt willen beperken tot wat vandaag runtime-gedrag verandert, kan deze constant ook weggelaten worden tot er effectief een PowerBI-endpoint of policy bijkomt.

### 5. Wrapper behouden geeft minder churn

In plaats van alle Backoffice-controllers massaal om te zetten naar `[Authorize(Policy = PolicyNames.BackofficeUser)]`, is `OrganisationRegistryAuthorizeAttribute` behouden als wrapper.

Voordelen:

- Minder controller churn.
- Bestaande role-based checks blijven intact.
- Een enkele centrale plek voor toekomstige Backoffice-auth wijzigingen.

### 6. Directe authenticate checks moesten mee

Een aantal plekken gebruikt na authorization nog `HttpContext.GetAuthenticateInfoAsync()` voor role/visibility checks. Die helper keek alleen naar `Bearer`.

Daarom is de helper uitgebreid naar:

1. probeer `Bearer`;
2. als dat niet lukt, probeer `BffApi`;
3. probeer alleen schemes die effectief geregistreerd zijn.

## Testdekking

Toegevoegde/aangepaste contracttests bewaken nu:

- Backoffice command controllers gebruiken `PolicyNames.BackofficeUser`.
- Backoffice command controllers declareren geen authentication schemes meer.
- `OrganisationRegistryAuthorizeAttribute` is een wrapper rond `BackofficeUser`.
- Role mapping in het attribuut blijft werken.
- `BackofficeUser` policy gebruikt alleen `Bearer` wanneer `BffApi` uit staat.
- `BackofficeUser` policy gebruikt `Bearer` en `BffApi` wanneer `BffApi` aan staat.
- `BackofficeUser` laat een geauthenticeerde gebruiker met `vo_id` toe.
- `BackofficeUser` weigert een M2M-token met alleen scopes.
- EditApi controllers blijven `AuthenticationSchemes.EditApi` en een scope-policy gebruiken.

## Uitgevoerde verificatie

Geslaagd:

```bash
dotnet test test/OrganisationRegistry.UnitTests/OrganisationRegistry.UnitTests.csproj --filter "FullyQualifiedName~OrganisationRegistry.UnitTests.Security.AuthorizationContractTests"
```

Resultaat: 12 passed.

Geslaagd:

```bash
dotnet test test/OrganisationRegistry.UnitTests/OrganisationRegistry.UnitTests.csproj --filter "FullyQualifiedName~OrganisationRegistry.UnitTests.Security"
```

Resultaat: 125 passed.

Geslaagd:

```bash
dotnet test test/OrganisationRegistry.UnitTests/OrganisationRegistry.UnitTests.csproj
```

Resultaat: 762 passed, 2 skipped.

Geprobeerd:

```bash
dotnet test OrganisationRegistry.sln --no-restore
```

Resultaat: faalt op lokale infrastructuurafhankelijkheden:

- ElasticSearch tests kunnen geen indices aanmaken.
- API integration tests kunnen geen SQL Server connectie openen.

Deze fouten zijn niet veroorzaakt door de policy-based auth wijziging.
