# API Integration Tests tegen Tilt

Deze nota documenteert de opvolgwerkzaamheden na de Keycloak demo-implementatie om de API integration tests niet langer lokaal een eigen host te laten starten, maar tegen de bestaande Tilt-stack te laten lopen.

## Doel

De oorspronkelijke integration tests startten zelf een API-host en gebruikten lokale shortcuts die niet meer overeenkwamen met de huidige demo-omgeving.

Dat gaf drie concrete problemen:

- de tests valideerden niet de echte Keycloak- en Tilt-integratie
- configuratie en seed-data liepen uit sync met de demo-stack
- een deel van de suite was flaky door timing, lokale poorten en fixture-aannames

De gekozen richting is daarom: test tegen de bestaande Tilt-omgeving, en verplaats alle teststabilisatie naar testcode, testconfig en demo-infra.

## Wat we nu hebben gedaan

### 1. `ApiFixture` test nu tegen Tilt in plaats van een eigen host te starten

Bestand:

- [test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs)

Wat is aangepast:

- de fixture start geen lokale API-host meer
- de basis-URL komt uit `ApiIntegrationTests:ApiBaseUrl`
- default endpoint is nu `http://api.localhost:9080`
- de fixture gebruikt echte externe dependencies uit Tilt:
  - API
  - WireMock
  - Keycloak
- machine-to-machine authenticatie blijft via echte Keycloak `client_credentials`

Waarom:

- de tests moeten dezelfde paden en configuratie gebruiken als de demo-stack
- auth-, routing- en introspectionproblemen worden zo zichtbaar in de echte vorm

### 2. Testconfig is uitgelijnd met de demo-seed

Bestanden:

- [test/OrganisationRegistry.Api.IntegrationTests/appsettings.json](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/appsettings.json)
- [test/OrganisationRegistry.Api.IntegrationTests/IntegrationTestConfigurationTests.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/IntegrationTestConfigurationTests.cs)
- [test/OrganisationRegistry.Api.IntegrationTests/appsettings.Oussamas-MacBook-Pro.json](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/appsettings.Oussamas-MacBook-Pro.json)

Wat is aangepast:

- test-endpoints wijzen nu naar `api.localhost`, `mock.localhost`, `opensearch.localhost` en de Keycloak realm in Tilt
- seed-afhankelijke ids zoals KBO- en Orafin-configuratie zijn gelijkgetrokken met de demo-seed
- de configtest bewaakt dat deze waarden niet opnieuw uit sync raken

Waarom:

- meerdere failures kwamen niet uit functioneel gedrag, maar uit verouderde testconfig
- door seed en tests op dezelfde ids te zetten, verdwijnen lokale afwijkingen

### 3. Tilt-infra is aangevuld voor het MAGDA/KBO-certificaat

Bestanden:

- [demo/k8s/secrets.yaml](/Users/oussamasadik/Documents/GitHub/organisation-registry/demo/k8s/secrets.yaml)
- [demo/k8s/api.yaml](/Users/oussamasadik/Documents/GitHub/organisation-registry/demo/k8s/api.yaml)
- [demo/k8s/seed.yaml](/Users/oussamasadik/Documents/GitHub/organisation-registry/demo/k8s/seed.yaml)
- [seed/seed.py](/Users/oussamasadik/Documents/GitHub/organisation-registry/seed/seed.py)

Wat is aangepast:

- het KBO-certificaat en het bijhorende password worden via k8s secrets aan de API en seed-job doorgegeven
- de seed schrijft deze waarden ook naar de configuratie in SQL Server
- de API init-container schrijft de MAGDA endpoints vóór API startup naar WireMock in de SQL-configuratietabel
- de seed houdt dezelfde WireMock endpoints idempotent goed

Waarom:

- zonder deze waarden viel de MAGDA/KBO-flow terug op een ongeldige placeholderconfiguratie
- dat gaf een `ClientCertificate should never be null` / ongeldige base64-cascade in de demo-omgeving
- Kubernetes env vars alleen zijn niet genoeg: de API laadt `[OrganisationRegistry].[Configuration]` uit SQL Server tijdens startup, en die waarden overschrijven de env-config
- daarom moeten `Api:KboMagdaEndpoint` en `Api:RepertoriumMagdaEndpoint` al vóór API startup op `http://wiremock:8080` staan

Belangrijke nuance:

- de tests mogen nooit naar de echte MAGDA HTTPS endpoints gaan
- de init-container voorkomt dat een verse Tilt-stack eerst met productieachtige MAGDA endpoints opstart en daarna pas door de seed gecorrigeerd wordt

### 4. Er is een pre-test readiness script toegevoegd

Bestand:

- [scripts/wait-for-tilt-api-integration-tests.sh](/Users/oussamasadik/Documents/GitHub/organisation-registry/scripts/wait-for-tilt-api-integration-tests.sh)

Wat dit doet:

- wacht tot de Tilt API beschikbaar is
- genereert een test-JWT voor backoffice-calls
- triggert indien nodig de Piavo-import
- wacht tot de relevante read models en demo-organisaties effectief beschikbaar zijn

Waarom:

- een deel van de failures kwam door timing: de API was al up, maar import of projecties nog niet
- deze readiness check haalt die race condition uit de teststart

### 5. Import/read-model stabilisatie zit nu in testharness en importtool

Bestanden:

- [test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs)
- [test/OrganisationRegistry.Api.IntegrationTests/WhenTheImportHasRun.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/WhenTheImportHasRun.cs)
- [test/OrganisationRegistry.Import.Piavo/Program.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Import.Piavo/Program.cs)

Wat is aangepast:

- de fixture vult ontbrekende demo-relaties idempotent aan waar Tilt-data anders te dun is
- de importtool is toleranter gemaakt voor reeds bestaande data en falende deelstappen
- de importtests vragen ongepagineerde resultaten op zodat seeded organisaties zichtbaar blijven

Waarom:

- de suite moet tegen een gedeelde demo-stack kunnen lopen zonder te veronderstellen dat alles schoon of exact hetzelfde is

### 6. Datumasserties zijn centraal gefixt in de testlaag

Bestand:

- [test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs)

Wat is aangepast:

- `Deserialize()` en `DeserializeAsList()` normaliseren de velden `date`, `validFrom` en `validTo`
- ISO-datumstrings worden daar omgezet naar `DateTime.Date`

Waarom:

- de API geeft JSON-datums als strings terug
- veel bestaande assertions verwachtten al `DateTime`-waarden
- deze fix zit nu op één centrale plek in de testharness, zonder productiesource of elk testbestand apart aan te passen

### 7. Edit API M2M-principal wordt correct geselecteerd

Bestanden:

- [src/OrganisationRegistry.Api/Infrastructure/Security/ConfigureClaimsPrincipalSelectorMiddleware.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/Security/ConfigureClaimsPrincipalSelectorMiddleware.cs)
- [src/OrganisationRegistry.Api/Security/SecurityService.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/src/OrganisationRegistry.Api/Security/SecurityService.cs)

Wat is aangepast:

- `ClaimsPrincipal.Current` gebruikt nu eerst de reeds geauthenticeerde `HttpContext.User`
- als fallback wordt voor Edit API-verkeer eerst het introspection scheme geprobeerd en pas daarna het gewone bearer scheme
- `SecurityService.GetRequiredUser()` herkent M2M-calls nu primair via `client_id` of `azp`
- `testClient` wordt expliciet gemapt naar `WellknownUsers.TestClient`, ook wanneer Keycloak meerdere scopes teruggeeft

Waarom:

- Edit API-endpoints autoriseren via introspection, maar command dispatch gebruikt nog `ClaimsPrincipal.Current`
- Keycloak access tokens zijn ook JWTs; daardoor probeerde het bearer scheme eerst te valideren en ontstond een foute/lege principal in de command flow
- `testClient` kreeg in Keycloak meerdere scopes terug, inclusief CJM-scope, waardoor scope-first mapping hem foutief als CJM behandelde

## Wat dit concreet heeft opgelost

Deze delen zijn intussen groen of expliciet gestabiliseerd:

- configuratietest voor de Tilt/seed-config
- import-readiness en de meeste `WhenTheImportHasRun`-checks
- datumvergelijkingen in backoffice- en edit-tests
- MAGDA-certificaatprobleem in de demo-omgeving
- MAGDA/KBO-calls in Tilt worden vóór API startup naar WireMock gerouteerd
- M2M-principal selectie voor Edit API command dispatch
- expliciete client mapping voor `cjmClient`, `orafinClient` en `testClient`
- `CreateFromKboNumberTests`, `CreateContactsTests` en `CreateOrganisationOrganisationClassificationTests`
- seed-job in Tilt eindigt opnieuw als `Completed`
- volledige backend test suite draait groen tegen de Tilt-stack

## Huidige verificatie

Laatste uitgevoerde checks:

- `./scripts/wait-for-tilt-api-integration-tests.sh`
  - resultaat: OK
- gerichte API integration run voor:
  - `CreateFromKboNumberTests`
  - `CreateContactsTests`
  - `CreateOrganisationOrganisationClassificationTests`
  - resultaat: 13 passed, 0 failed
- `dotnet test --no-restore`
  - resultaat: exit code 0

Belangrijkste projectresultaten uit de volledige run:

- `OrganisationRegistry.UnitTests`: 776 passed, 2 skipped
- `OrganisationRegistry.KboMutations.UnitTests`: 27 passed
- `OrganisationRegistry.SqlServer.IntegrationTests`: 32 passed
- `OrganisationRegistry.ElasticSearch.Tests`: 95 passed, 1 skipped
- `OrganisationRegistry.Api.IntegrationTests`: 89 passed, 1 skipped

Bekende niet-failure meldingen:

- `OrganisationRegistry.Tests.Shared` en `OrganisationRegistry.VlaanderenBeNotifier.UnitTests` geven `No test is available`; dit zijn geen falende tests in deze run.

## Wat nog openstaat

Er is momenteel geen bekende resterende blocker voor de backend test suite tegen Tilt.

Als er later opnieuw failures opduiken, moet de analyse opnieuw onderscheid maken tussen:

- echte API-regressies
- ontbrekende demo-seeddata
- gedeelde Tilt-state/idempotency
- projectie/readiness timing

## Eerdere analyse van de falende auth-test

Betrokken code:

- [src/OrganisationRegistry.Api/Security/SecurityService.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/src/OrganisationRegistry.Api/Security/SecurityService.cs)
- [src/OrganisationRegistry.Api/Edit/Organisation/KboNumber/OrganisationsController.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/src/OrganisationRegistry.Api/Edit/Organisation/KboNumber/OrganisationsController.cs)

Waargenomen gedrag voor de fix:

- `CreateFromKboNumberTests` eindigt op `500 Internal Server Error`
- de eerdere MAGDA certificate failure is weg
- de request loopt nu vast in `SecurityService`
- de exception is: `Could not determine current user's first name`
- in de logs is daarnaast bearer-validatie/claiminterpretatie te zien rond deze flow

Interpretatie:

- de endpoint-policy zelf slaagde via introspection
- command dispatch gebruikte daarna `ClaimsPrincipal.Current`
- de selector probeerde het bearer scheme vóór introspection, terwijl Keycloak M2M-tokens via introspection bedoeld zijn
- daardoor zag `SecurityService.GetRequiredUser()` geen correct herkende M2M-principal en viel terug op human claims

Aanvullende bevinding:

- `testClient` kreeg van Keycloak meerdere scopes terug, inclusief `dv_organisatieregister_cjmbeheerder`
- scope-first mapping koos daardoor foutief `WellknownUsers.Cjm`
- client-id mapping is betrouwbaarder voor well-known M2M-clients

## Besluit

De teststrategie is nu expliciet verschoven van "start lokaal een eigen service" naar "test tegen de echte Tilt-demo-omgeving".

Dat is belangrijk omdat het:

- de demo realistischer maakt
- configuratieverschillen sneller zichtbaar maakt
- auth- en integratieproblemen op de juiste plek laat falen

De eerder resterende `CreateFromKboNumberTests`, `CreateContactsTests` en organisation-classification failures zijn opgelost. De volledige backend suite is nadien succesvol uitgevoerd tegen de draaiende Tilt-stack.
