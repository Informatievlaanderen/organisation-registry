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
- [demos/seed/Program.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/demos/seed/Program.cs)

Wat is aangepast:

- het KBO-certificaat en het bijhorende password worden via k8s secrets aan de API en seed-job doorgegeven
- de seed schrijft deze waarden ook naar de configuratie in SQL Server

Waarom:

- zonder deze waarden viel de MAGDA/KBO-flow terug op een ongeldige placeholderconfiguratie
- dat gaf een `ClientCertificate should never be null` / ongeldige base64-cascade in de demo-omgeving

Belangrijke nuance:

- in de draaiende omgeving moest de bestaande SQL-config nog manueel gecorrigeerd worden omdat de reeds gebouwde seed-jobimage deze wijziging nog niet bevatte
- de broncode staat nu wel goed; een verse build/deploy moet die manuele stap overbodig maken

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

## Wat dit concreet heeft opgelost

Deze delen zijn intussen groen of expliciet gestabiliseerd:

- configuratietest voor de Tilt/seed-config
- import-readiness en de meeste `WhenTheImportHasRun`-checks
- datumvergelijkingen in backoffice- en edit-tests
- MAGDA-certificaatprobleem in de demo-omgeving

## Wat nog openstaat

De belangrijkste resterende failure is:

- `CreateFromKboNumberTests`

Dat is nu geen certificaatprobleem meer. Na de infra-fix verschuift de fout naar authenticatie/claimsverwerking op de Edit API-flow.

## Huidige analyse van de falende test

Betrokken code:

- [src/OrganisationRegistry.Api/Security/SecurityService.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/src/OrganisationRegistry.Api/Security/SecurityService.cs)
- [src/OrganisationRegistry.Api/Edit/Organisation/KboNumber/OrganisationsController.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/src/OrganisationRegistry.Api/Edit/Organisation/KboNumber/OrganisationsController.cs)

Waargenomen gedrag:

- `CreateFromKboNumberTests` eindigt op `500 Internal Server Error`
- de eerdere MAGDA certificate failure is weg
- de request loopt nu vast in `SecurityService`
- de exception is: `Could not determine current user's first name`
- in de logs is daarnaast bearer-validatie/claiminterpretatie te zien rond deze flow

Interpretatie:

- deze endpoint verwacht nog steeds een bruikbare gebruiker of correct herkende M2M-identiteit
- de huidige claims in deze specifieke flow worden niet op dezelfde manier herkend als in de andere reeds gefixte Edit API-calls
- dit is dus een afzonderlijk auth/claims-probleem, niet langer een Tilt-infra- of certificate-probleem

## Wat nog moet gebeuren

Voor de resterende test is het volgende werk nog nodig:

1. Reproduceer `CreateFromKboNumberTests` met API-logs erbij.
2. Vergelijk de claims principal van deze request met een reeds werkende Edit API M2M-call.
3. Bepaal of de fout zit in:
   - introspection-resultaat
   - scope parsing
   - clientherkenning als M2M-caller
   - vereiste naamclaims op een pad dat eigenlijk M2M-safe zou moeten zijn
4. Fix dit in de auth/claims-afhandeling van de API, niet in de test.
5. Her-run daarna minstens:
   - `CreateFromKboNumberTests`
   - de andere Edit API integration tests

## Besluit

De teststrategie is nu expliciet verschoven van "start lokaal een eigen service" naar "test tegen de echte Tilt-demo-omgeving".

Dat is belangrijk omdat het:

- de demo realistischer maakt
- configuratieverschillen sneller zichtbaar maakt
- auth- en integratieproblemen op de juiste plek laat falen

De resterende `CreateFromKboNumberTests` failure is nog relevant, maar is nu teruggebracht tot één duidelijk afgebakend probleem: de auth/claims-verwerking van die specifieke KBO create-flow.
