# Implementation Plan: Zoeken op code (OVO, KBO, Orafin) op de hoofdpagina

**Branch**: `feat/or-3785-wegwijs-code-search` | **Date**: 2026-11-10 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/011-wegwijs-code-search/spec.md`
**Jira**: OR-3785

## Summary

De hoofdpagina van Wegwijs krijgt een filterveld **Code** dat in één invoerveld zoekt op
OVO-nummer, KBO-nummer en Orafin-sleutelwaarde (deelmatch op genormaliseerde waarde),
waarbij **alle** Orafin-sleutels meetellen, ook vervallen.
Daarvoor komt er een **eigen read model**: een kopie van de `OrganisationList`-projectie,
uitgebreid met een zustertabel die alle doorzoekbare codes van een organisatie bevat
(herkomst, sleuteltype en geldigheidsperiode).

De technische kern van de aanpak: **de codes worden met hun sleuteltype geprojecteerd, maar de
beoordeling "welk sleuteltype is Orafin" gebeurt op zoekmoment.** Daardoor vervalt het probleem
van het dynamische Orafin-sleuteltype: wijzigt de instelling `Api:Orafin_KeyTypeId`, dan volgen de
resultaten na een herstart van de API, zonder rebuild.

De geldigheidsperiode van een sleutel speelt geen rol (FR-005b): alle Orafin-sleutels van een
organisatie zijn doorzoekbaar, ook vervallen. Daardoor is het zoekresultaat datum-onafhankelijk en
is er geen enkele tijdsgebonden logica nodig — geen nachtelijke taak, geen rebuild bij een
verstrijkende datum.

## Technical Context

**Language/Version**: C# / .NET 10 (nullable reference types enabled)
**Primary Dependencies**: Entity Framework Core, FluentValidation, Serilog, Autofac; Angular (UI)
**Storage**: SQL Server — eventstore + read models in schema `OrganisationRegistry` (backoffice)
**Testing**: xUnit; `ListViewTestBase` (in-memory EF) voor projectietests, `OrganisationRegistry.Api.IntegrationTests` voor endpoint-tests, `OrganisationRegistry.ArchitectureTests` (ArchUnitNET) voor conventies
**Target Platform**: Linux containers (API + projectiehosts)
**Project Type**: Web service (CQRS read side) + Angular beheer-UI
**Performance Goals**: geen merkbare regressie op de overzichtslijst; code-filter moet index-gestuurd zijn (sargable, geen functies op kolommen)
**Constraints**: geen nieuwe ElasticSearch-index (C-003); bestaande `OrganisationList`-projectie en haar consumenten blijven ongewijzigd (C-001); Orafin via geconfigureerd sleuteltype-id (C-002)
**Scale/Scope**: ±1 nieuwe projectie (3 tabellen gekopieerd + 1 nieuwe codetabel), 1 nieuwe query + endpoint, 1 UI-filterveld, 1 EF-migratie

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principe | Beoordeling |
|---|---|
| I. Event sourcing is immutable law | ✅ Geen nieuwe of gewijzigde events; enkel een nieuw read model dat uit de bestaande eventstroom wordt opgebouwd. |
| II. Commands and events drive all state changes | ✅ Read-only feature; geen commands, geen aggregaatwijzigingen. |
| III. CQRS: command en query strikt gescheiden | ✅ Volledig aan de querykant: nieuwe SQL-projectie + query. Geen enkele schrijfpad-wijziging. |
| IV. Respect aggregate boundaries | ✅ De cross-aggregaat-informatie (organisatie + sleutels + sleuteltype + KBO) wordt via de projectie samengebracht, niet door aggregaten te doorlopen. |
| V. Testing is not optional | ✅ Projectietests per event (`ListViewTestBase`), querytests op het filtergedrag, integratietest op het endpoint, architectuurtest tegen event-drift tussen beide projecties. |
| Read models zijn disposable | ✅ Nieuwe projectie erft `Projection<T>` en is manueel herbouwbaar via `POST /v1/tasks` met `{"type":"RebuildProjection","params":["<ProjectionFullName>"]}` (permissie `CanRunScheduledJobs`). |
| Domeintaal Nederlands | ✅ UI-label "Code"; domeinbegrippen (OVO-nummer, KBO, Orafin-sleutel) onvertaald; codenamen volgen de bestaande Engelse klassenamen van het read model. |
| Migraties nooit herschrijven | ✅ Één nieuwe migratie, geen bestaande migratie aangepast. |

Geen afwijkingen die rechtvaardiging vereisen, met één aandachtspunt dat in
[Complexity Tracking](#complexity-tracking) staat: de bewuste duplicatie van projectielogica.

## Project Structure

### Documentation (this feature)

```text
specs/011-wegwijs-code-search/
├── spec.md              # Feature specificatie (Approved)
├── plan.md              # Dit bestand
├── data-model.md        # Tabellen, kolommen, indexen, normalisatieregels
└── tasks.md             # Taakafbakening (speckit.tasks)
```

Er is geen aparte `research.md`: de verkenning (welke projectie de hoofdpagina bedient, waar
KBO-nummer en sleutels vandaan komen, hoe Orafin vandaag bepaald wordt) is verwerkt in
[Key Decisions](#key-decisions) en [Reference Points](#reference-points-bestaande-code).
`quickstart.md` en `contracts/` zijn niet van toepassing: het contract is één extra filterveld,
gedocumenteerd onder [API-contract](#api-contract).

### Source Code (repository root)

```text
src/
├── OrganisationRegistry.SqlServer/
│   ├── Organisation/
│   │   ├── OrganisationListItemView.cs                  # bron van de kopie (ongewijzigd)
│   │   ├── OrganisationKeyListItemView.cs               # referentiepatroon sleutel-events
│   │   ├── OrganisationSearchListItemView.cs            # NIEUW: kopie-projectie + codetabel
│   │   └── OrganisationSearchCode.cs                    # NIEUW: entiteit + EF-configuratie codetabel
│   ├── Infrastructure/
│   │   └── OrganisationRegistryContext.cs               # NIEUWE DbSets
│   └── Migrations/
│       └── <timestamp>_AddOrganisationSearchList.cs     # NIEUW
├── OrganisationRegistry.Api/
│   └── Backoffice/Organisation/List/
│       ├── OrganisationListQuery.cs                     # ongewijzigd
│       ├── OrganisationSearchListQuery.cs               # NIEUW: filter incl. code
│       └── OrganisationListController.cs                # NIEUW endpoint-actie
└── OrganisationRegistry.UI/app/
    ├── services/organisations/organisation-filter.model.ts   # veld code
    ├── services/organisations/organisation.service.ts        # nieuw endpoint
    └── organisation/overview/filter/                         # invoerveld Code

test/
├── OrganisationRegistry.SqlServer.IntegrationTests/OnProjections/OrganisationSearch/   # NIEUW
├── OrganisationRegistry.Api.IntegrationTests/BackOffice/Organisation/                  # NIEUW
└── OrganisationRegistry.ArchitectureTests/                                             # drift-guard
```

**Structure Decision**: De feature blijft binnen de bestaande architectuurgrenzen: read model en
projectie in `OrganisationRegistry.SqlServer/Organisation/`, query en endpoint in
`OrganisationRegistry.Api/Backoffice/Organisation/List/`, UI-filter in de bestaande
overzicht-filtercomponent. Geen nieuw project, geen nieuwe host: SQL-projecties worden
automatisch geregistreerd via de assembly-scan in
`src/OrganisationRegistry.Api/Infrastructure/Configuration/UseWegwijsEventSourcing.cs:28`.

## Key Decisions

### D-001 — Codes als rijen in een zustertabel, niet als kolom

Eén tabel `OrganisationSearchCodeList`, één rij per doorzoekbare code, met `Source`
(`Ovo` / `Kbo` / `Key`), `KeyTypeId` (null voor OVO en KBO), de ruwe waarde, de genormaliseerde
waarde en `ValidFrom`/`ValidTo`.

*Waarom*: één uniforme index en één OR-term in de query, en het model is uitbreidbaar als er later
nog een codetype bij moet. Een gematerialiseerde `Code`-kolom per organisatie zou een
samengestelde waarde moeten bevatten en zou voor elke organisatie één lange samengestelde string vergen waarin je niet per bron kunt filteren (Orafin-sleuteltype, FR-007).

**Alle sleuteltypes worden geprojecteerd, niet enkel Orafin.** Dat is wat C-002 en FR-007 samen
mogelijk maakt: de projectie blijft config-onafhankelijk, de query filtert op het geconfigureerde
`Api:Orafin_KeyTypeId`.

### D-002 — Geen geldigheidstoets; het resultaat is datum-onafhankelijk

Het `code`-filter toetst `ValidFrom`/`ValidTo` van een sleutel **niet** (FR-005b). Alle
Orafin-sleutels van een organisatie zijn doorzoekbaar, ook vervallen en toekomstige.

De kolommen `ValidFrom`/`ValidTo` blijven wel in het read model staan, maar enkel informatief:
ze komen gratis uit het event mee, maken een rij traceerbaar, en laten een latere beleidswijziging
("toch enkel geldige sleutels") een wijziging in de query zijn in plaats van een herprojectie.

*Waarom geen toets*: het verstrijken van een `ValidTo` genereert geen event. Elke vorm van
geldigheidsafhankelijkheid maakt het zoekresultaat tijdsgebonden — ofwel met een stille fout in de
projectie, ofwel met een datumvergelijking in elke query. Zonder toets is er niets tijdsgebonden
te onderhouden. De bestaande filter "enkel actieve organisaties" blijft het enige tijdsgebonden
criterium en werkt op de geldigheid van de organisatie (FR-005c).

### D-003 — Normalisatie met kandidaat-zoektermen

Opgeslagen kant: `NormalizedValue = Normalize(value)` met `Normalize` = trimmen, naar hoofdletters
(invariant), en alle niet-alfanumerieke tekens verwijderen. Daarmee wordt `0123.456.789` →
`0123456789` en `ovo001234` → `OVO001234`.

Zoekkant: uit de ingegeven term wordt een kleine verzameling kandidaten gemaakt
(`Normalize(term)`, plus de variant zonder `BE`-landcode als de term daarop lijkt). De query doet
voor elke kandidaat `c.NormalizedValue.Contains(kandidaat)` → `LIKE '%kandidaat%'`, met een OR
tussen de kandidaten (hoogstens twee).

*Waarom*: het ticket vraagt deelmatch tijdens het typen. De zoekterm wordt genormaliseerd, niet de
kolom, dus er staat geen functie op de kolom. De voorloopnul-variant voor 9-cijferige termen is
overbodig geworden: `123456789` zit al in `0123456789`. De `BE`-variant blijft nodig, want
`BE0123456789` zit niet in `0123456789`.

*Afweging*: een leidende wildcard kan de index niet gebruiken voor een seek; SQL Server scant de
(smalle) index op `NormalizedValue`. Dat is aanvaardbaar zolang de codetabel klein blijft, maar
moet bevestigd worden met T035 (SC-005).

### D-004 — Eigen endpoint voor de hoofdpagina *(beslist: `overview`)*

`GET /v1/organisations/overview` (nieuwe actie op `OrganisationListController`) bedient de
hoofdpagina en leest uit het nieuwe read model. Het bestaande `GET /v1/organisations` en de
`OrganisationList`-projectie blijven letterlijk ongewijzigd, en blijven de bestaande consumenten
(onder andere de organisatie-autocomplete) bedienen.

*Waarom*: dat is de isolatie die C-001 beoogt, en het garandeert FR-009 zonder regressierisico.
Het bestaande `GET /v1/organisations` blijft de generieke lijst voor de overige consumenten
(onder andere de organisatie-autocomplete); `overview` is expliciet het endpoint van de
hoofdpagina. *Alternatief dat verworpen is*: het bestaande endpoint uitbreiden en naar het nieuwe
read model laten lezen — minder code, maar dan hangen álle consumenten aan de nieuwe projectie.

### D-005 — Drift-guard in plaats van gedeelde basisklasse

Beide projecties blijven onafhankelijke klassen (C-001). Om te vermijden dat de kopie stil
achterop raakt, komt er een architectuurtest die vaststelt dat de nieuwe projectie minstens de
eventverzameling van `OrganisationListItemView` afhandelt.

*Waarom*: een gedeelde generieke basisklasse zou de twee read models opnieuw koppelen en de
rebuild-semantiek (`Projection<T>` matcht op `typeof(T).FullName`) ingewikkelder maken.

## API-contract

Nieuw endpoint, zelfde filter-/sorteer-/pagineringsmechaniek als het bestaande overzicht
(`x-filtering`, `x-sorting`, `x-pagination` headers):

```text
GET /v1/organisations/overview
x-filtering: {"name":"", "ovoNumber":"", "code":"0123.456.789",
              "activeOnly":true, "formalFrameworkId":"", 
              "organisationClassificationTypeId":"", "organisationClassificationId":"",
              "authorizedOnly":false}
```

- `code` is optioneel; leeg of enkel witruimte = niet ingevuld.
- Responsvorm, kolommen en sortering zijn identiek aan `GET /v1/organisations` (FR-006).
- Route: `[HttpGet("overview")]` op de bestaande `OrganisationListController` (`OrganisationRegistryRoute("organisations")`).
- Autorisatie en zichtbaarheidsregels zijn identiek aan het bestaande overzicht (FR-010).

## Reference Points (bestaande code)

| Wat | Waar |
|---|---|
| Te kopiëren projectie (3 tabellen) | `src/OrganisationRegistry.SqlServer/Organisation/OrganisationListItemView.cs` |
| Patroon sleutel-events + `KeyTypeUpdated`/`KeyTypeRemoved`/`ScheduledForRemoval` | `src/OrganisationRegistry.SqlServer/Organisation/OrganisationKeyListItemView.cs` |
| Patroon KBO-nummer uit events | `src/OrganisationRegistry.SqlServer/Organisation/OrganisationDetailItemView.cs:195` |
| Bestaand filter (naam, OVO, activeOnly, toepassingsgebied, classificatie) | `src/OrganisationRegistry.Api/Backoffice/Organisation/List/OrganisationListQuery.cs:105` |
| Orafin-sleuteltype uit configuratie | `src/OrganisationRegistry.Infrastructure/Configuration/OrganisationRegistryConfiguration.cs:24`, gebruikt in `FormalFrameworkOrganisationExtended.cs:88` |
| `IOrganisationRegistryConfiguration` in een query injecteren | `src/OrganisationRegistry.Api/Backoffice/Parameters/LocationType/Queries/LocationTypeListQuery.cs:22` |
| Automatische registratie van SQL-projecties | `src/OrganisationRegistry.Api/Infrastructure/Configuration/UseWegwijsEventSourcing.cs:28` |
| EF-configuraties worden per assembly gescand | `src/OrganisationRegistry.SqlServer/Infrastructure/OrganisationRegistryContext.cs:190` |
| Projectielijst (namen voor de rebuild) | `src/OrganisationRegistry.Api/Backoffice/Admin/Projections/ProjectionsController.cs` |
| Rebuild manueel starten | `src/OrganisationRegistry.Api/Backoffice/Admin/Task/TaskController.cs:54` (`POST /v1/tasks`, `TaskType.RebuildProjection`) |
| Testbasis voor projectietests | `test/OrganisationRegistry.SqlServer.IntegrationTests/TestBases/ListViewTestBase.cs` |
| UI-filter hoofdpagina | `src/OrganisationRegistry.UI/app/organisation/overview/filter/filter.component.ts` + `filter.template.html` |

## Implementation Phases

**Phase 1 — Read model en migratie**
Kopie-entiteiten + EF-configuraties + codetabel, `DbSet`s in de context, één EF-migratie.
Levert nog geen gedrag; maakt de rest testbaar.

**Phase 2 — Projectie**
Kopie van `OrganisationListItemView` naar `OrganisationSearchListItemView` (identiek gedrag voor
de bestaande kolommen) plus de code-events: OVO bij creatie, KBO-koppeling en -opzegging,
sleutels toevoegen/wijzigen/verwijderen, sleuteltype verwijderen, beëindiging.
Projectietests per event. Dekt US3.

**Phase 3 — Query en endpoint**
`OrganisationSearchListQuery` met het `code`-filter (normalisatie, Orafin-sleuteltype uit
configuratie, geen geldigheidstoets), nieuwe controller-actie, integratietests.
Dekt US1 en US2 aan de API-kant.

**Phase 4 — UI**
Filterveld **Code** op de hoofdpagina en in het filtermodel; overzicht leest van het nieuwe
endpoint. De `atLeastOne`-validatie wordt op deze filter niet gebruikt, dus zoeken met enkel Code
werkt zonder aanpassing (T022).

**Phase 5 — Oplevering**
Drift-guard-architectuurtest, regressiecheck op het bestaande overzicht, documentatie van het
nieuwe endpoint in Swagger. De API herbouwt bij het opstarten de projecties uit de whitelist
`Projections:InitialiseWhenEmpty` wanneer al hun tabellen leeg zijn en de event store niet
(`InitialiseEmptyProjectionsExtension`); de nieuwe projectie staat in die whitelist, zodat er per
omgeving geen manuele rebuild nodig is. In de Tilt-omgeving is `Api:Orafin_KeyTypeId` ingesteld
(`demo/k8s/apps/api.yaml`, `scripts/seed-tilt-api-configuration.sh`).

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Duplicatie van de projectielogica (`OrganisationSearchListItemView` naast `OrganisationListItemView`) | Eis uit OR-3785 (C-001): de hoofdpagina-zoekfunctie krijgt een eigen read model, zodat de bestaande lijst en haar consumenten niet raken aan deze wijziging | De codetabel toevoegen aan het bestaande `OrganisationList` zou minder code zijn, maar wijzigt een read model met meerdere bestaande consumenten. Een gedeelde generieke basisklasse koppelt beide read models opnieuw en bemoeilijkt de rebuild-semantiek. Drift wordt opgevangen met de architectuurtest uit D-005 |
| Alle sleuteltypes in het read model terwijl enkel Orafin doorzoekbaar is | Maakt het read model onafhankelijk van de instelling `Api:Orafin_KeyTypeId` en vervult daarmee FR-007 zonder rebuild | Enkel de Orafin-sleutel projecteren vereist een projectie-rebuild bij elke wijziging van de instelling — wat FR-007 uitsluit |
