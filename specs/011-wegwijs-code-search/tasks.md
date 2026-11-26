---
description: "Taaklijst voor OR-3785 — zoeken op code (OVO, KBO, Orafin) op de hoofdpagina"
---

# Tasks: Zoeken op code (OVO, KBO, Orafin) op de hoofdpagina

**Input**: `specs/011-wegwijs-code-search/` — [spec.md](./spec.md), [plan.md](./plan.md), [data-model.md](./data-model.md)
**Branch**: `feat/or-3785-wegwijs-code-search` | **Jira**: OR-3785

**Tests**: verplicht. De constitutie (principe V) eist unit-/projectietests én integratietests
voor nieuwe functionaliteit; schrijf de falende test eerst.

## Format: `[ID] [P?] [Story] Beschrijving`

- **[P]**: kan parallel met andere `[P]`-taken (andere bestanden, geen afhankelijkheid)
- **[Story]**: bijhorende user story uit spec.md (US1, US2, US3)

---

## Phase 1: Foundational (blokkerend voor alle stories)

**Doel**: het eigen read model bestaat, is gevuld voor de bestaande kolommen en is herbouwbaar.
Nog geen zichtbaar gedrag.

- [x] T001 Maak `OrganisationSearchListItem` + `OrganisationSearchListConfiguration` (kopie van `OrganisationListItem`, tabel `OrganisationSearchList`) in `src/OrganisationRegistry.SqlServer/Organisation/OrganisationSearchListItemView.cs`, incl. de twee kindentiteiten `OrganisationSearchFormalFrameworkValidity` en `OrganisationSearchClassificationValidity` met dezelfde indexen en cascade delete als het origineel (zie data-model.md §1)
- [x] T002 Maak `OrganisationSearchCodeListItem` + EF-configuratie (tabel `OrganisationSearchCodeList`, indexen `(NormalizedValue, KeyTypeId)`, `(OrganisationId)`, `(OrganisationKeyId)`) in `src/OrganisationRegistry.SqlServer/Organisation/OrganisationSearchCode.cs` (data-model.md §2)
- [x] T003 Voeg de `DbSet`s toe aan `src/OrganisationRegistry.SqlServer/Infrastructure/OrganisationRegistryContext.cs` (`OrganisationSearchList`, `OrganisationSearchFormalFrameworkValidity`, `OrganisationSearchClassificationValidity`, `OrganisationSearchCodeList`) — EF-configuraties worden automatisch gescand (`OrganisationRegistryContext.cs:190`)
- [x] T004 Genereer de EF-migratie `AddOrganisationSearchList` (`cd src/OrganisationRegistry.SqlServer && dotnet ef migrations add AddOrganisationSearchList --context OrganisationRegistryContext --startup-project ../OrganisationRegistry.Api/`) en controleer dat enkel de vier nieuwe tabellen in de up-migratie staan
- [x] T005 Maak `OrganisationSearchListItemView` als projectieklasse: kopie van de eventhandling van `src/OrganisationRegistry.SqlServer/Organisation/OrganisationListItemView.cs` voor de bestaande kolommen, met `ProjectionTables`-enum voor de nieuwe tabelnamen en `Schema = WellknownSchemas.BackofficeSchema`. Registratie gebeurt automatisch via de assembly-scan (`UseWegwijsEventSourcing.cs:28`)
- [x] T006 [P] Projectietest die vaststelt dat de gekopieerde projectie voor `OrganisationCreated`, `OrganisationNameUpdated`/`OrganisationShortNameUpdated`/`OrganisationValidityUpdated` (i.p.v. het obsolete `OrganisationInfoUpdated`), `OrganisationParentAdded` + `ParentAssignedToOrganisation`, `OrganisationOrganisationClassificationAdded` en `OrganisationFormalFrameworkAdded` dezelfde rijen oplevert als `OrganisationList` — `test/OrganisationRegistry.SqlServer.IntegrationTests/OnProjections/OrganisationSearch/OrganisationSearchListItemViewTests.cs` (basis: `TestBases/ListViewTestBase.cs`)
- [x] T007 [P] Verifieer dat de nieuwe projectie in `GET /v1/projections` verschijnt, zodat ze manueel herbouwd kan worden via `POST /v1/tasks` (`{"type":"RebuildProjection","params":["OrganisationRegistry.SqlServer.Organisation.OrganisationSearchListItemView"]}`) (FR-011)

---

## Phase 2: User Story 1 — Zoeken op één code zonder te weten welk type (P1)

**Doel**: één ingegeven code vindt de organisatie, ongeacht of het een OVO-nummer, KBO-nummer of
Orafin-code is.
**Independent test**: zoek achtereenvolgens op een gekend OVO-nummer, KBO-nummer en Orafin-code;
elke keer verschijnt exact de verwachte organisatie.

### Normalisatie

- [x] T008 [US1] Test eerst: unit tests voor `OrganisationCodeNormalizer` (`Normalize` en `Candidates`) met de gevallen uit data-model.md §4 — `0123.456.789`, `BE0123456789`, 9 cijfers (geen padding meer), `ovo001234`, witruimte, lege invoer — in `test/OrganisationRegistry.UnitTests/`
- [x] T009 [US1] Implementeer `OrganisationCodeNormalizer` (trim, hoofdletters invariant, niet-alfanumerieke tekens weg; kandidaatlijst met BE-prefix-variant) in `src/OrganisationRegistry.SqlServer/Organisation/` zodat zowel projectie als query hem gebruiken

### Codes projecteren (aanmaakpaden)

- [x] T010 [US1] Test eerst: projectietests voor het aanmaken van code-rijen bij `OrganisationCreated`, `OrganisationCreatedFromKbo`, `OrganisationCoupledWithKbo` en `OrganisationKeyAdded` — `test/OrganisationRegistry.SqlServer.IntegrationTests/OnProjections/OrganisationSearch/OrganisationSearchCodeListTests.cs`
- [x] T011 [US1] Implementeer in `OrganisationSearchListItemView` de `Ovo`-rij bij `OrganisationCreated` en `OrganisationCreatedFromKbo` (data-model.md §3)
- [x] T012 [US1] Implementeer de `Kbo`-rij bij `OrganisationCreatedFromKbo` en `OrganisationCoupledWithKbo` (patroon `OrganisationDetailItemView.cs:195`)
- [x] T013 [US1] Implementeer de `Key`-rij bij `OrganisationKeyAdded`, met `KeyTypeId`, `OrganisationKeyId`, waarde, genormaliseerde waarde en (informatief) `ValidFrom`/`ValidTo` uit het event — **alle** sleuteltypes, niet enkel Orafin (plan D-001)

### Query en endpoint

- [x] T014 [US1] Test eerst: querytests op het `code`-filter — deelmatch (begin, midden, einde van een code, ook `4`), match op OVO, op KBO (alle drie de schrijfwijzen), op een Orafin-sleutel, **ook match op een vervallen en op een toekomstige Orafin-sleutel** (FR-005b), géén match op een sleutel van een ander sleuteltype, leeg resultaat bij onbekende code
- [x] T015 [US1] Maak `OrganisationSearchListQuery` in `src/OrganisationRegistry.Api/Backoffice/Organisation/List/OrganisationSearchListQuery.cs`: kopie van `OrganisationListQuery` met `Code` in de filterklasse en het code-filter uit data-model.md §5 (zonder datumvergelijking); injecteer `IOrganisationRegistryConfiguration` (patroon `LocationTypeListQuery.cs:22`) voor `OrafinKeyTypeId`
- [x] T016 [US1] Voeg de actie `GET /v1/organisations/overview` toe aan `src/OrganisationRegistry.Api/Backoffice/Organisation/List/OrganisationListController.cs`, met dezelfde filtering-/sorting-/pagineringheaders en dezelfde autorisatie als de bestaande actie (plan: API-contract). Laat `GET /v1/organisations` ongemoeid (FR-009)
- [x] T017 [US1] *(gedraaid tegen de Tilt-omgeving: geslaagd)* Integratietest op het endpoint: zoeken op elk van de drie codetypes levert de juiste organisatie, en de responsvorm is identiek aan `GET /v1/organisations` (FR-006) — `test/OrganisationRegistry.Api.IntegrationTests/BackOffice/Organisation/`

### UI

- [x] T018 [US1] Voeg `code` toe aan `src/OrganisationRegistry.UI/app/services/organisations/organisation-filter.model.ts` (veld + `withCode` + `copy`)
- [x] T019 [US1] Laat het overzicht het nieuwe endpoint gebruiken in `src/OrganisationRegistry.UI/app/services/organisations/organisation.service.ts`
- [x] T020 [US1] Voeg het invoerveld **Code** toe aan `src/OrganisationRegistry.UI/app/organisation/overview/filter/filter.component.ts` + `filter.template.html` (label `Code`, placeholder `OVO-nummer, KBO-nummer of Orafin-code`), direct onder OVO-nummer

**Checkpoint**: na deze fase is US1 volledig testbaar en opleverbaar.

---

## Phase 3: User Story 2 — Code combineren met de bestaande filtervelden (P2)

**Doel**: Code gedraagt zich als een volwaardig filtercriterium naast de bestaande velden.
**Independent test**: zoek op een code van een beëindigde organisatie, met en zonder "enkel
actieve organisaties".

- [x] T021 [US2] Test eerst: querytests voor de combinaties — code + naam (AND), code + `activeOnly`, code + toepassingsgebied, code + classificatie(type), en één rij per organisatie bij meerdere matchende codes (FR-003, FR-005)
- [x] T022 [US2] *(vastgesteld: geen validatie toegevoegd, enkel getest)* Nagaan en vastleggen: de organisatie-overzichtsfilter gebruikt `atLeastOne` vandaag **niet** (de import in `filter.component.ts` is ongebruikt), dus zoeken met enkel Code werkt al. Bevestigen met een test i.p.v. validatie toe te voegen (FR-012, US2-scenario 3)
- [x] T023 [US2] *(gedraaid tegen de Tilt-omgeving: geslaagd)* Integratietest: enkel `code` ingevuld levert een geldige respons; `code` leeg laat het gedrag van de overige filters onveranderd (US2-scenario 4)

**Checkpoint**: Code is een volwaardig onderdeel van het filter.

---

## Phase 4: User Story 3 — Codes blijven correct na wijzigingen in de bron (P2)

**Doel**: wijzigingen aan sleutels, KBO-koppeling, sleuteltypes en beëindigingen werken door in
het zoekgedrag.
**Independent test**: sleutel toevoegen → gevonden; sleutel verwijderen → niet meer gevonden.

- [x] T024 [US3] Test eerst: projectietests voor `OrganisationKeyUpdated` (waarde én `ValidFrom`/`ValidTo`), `OrganisationKeyRemoved`, `KeyTypeRemoved`, `OrganisationCouplingWithKboCancelled`, `OrganisationTerminationSyncedWithKbo`, `OrganisationTerminated`/`OrganisationTerminatedV2` (incl. `FieldsToTerminateV2.Keys`)
- [x] T025 [US3] Implementeer `OrganisationKeyUpdated` en `OrganisationKeyRemoved` op de codetabel (patroon `OrganisationKeyListItemView.cs`)
- [x] T026 [US3] Implementeer `KeyTypeRemoved`: codes van dat sleuteltype verdwijnen
- [x] T027 [US3] Implementeer de KBO-paden: `OrganisationCouplingWithKboCancelled` (rij weg), `OrganisationTerminationSyncedWithKbo` en `OrganisationTerminated`/`OrganisationTerminatedV2` (rij weg of `ValidTo` bijwerken, zelfde voorwaarden als `OrganisationDetailItemView.cs:480`). Let op: een bijgewerkte `ValidTo` op een sleutelrij sluit die rij **niet** uit van het zoeken (FR-005b)
- [x] T028 [US3] Test dat `KeyTypeUpdated` (hernoemen) géén invloed heeft op het zoekgedrag — matchen gebeurt op `KeyTypeId` (C-002, data-model.md §3)
- [x] T029 [US3] Test dat een vervallen sleutel wél gevonden wordt, dat een beëindigde sleutel na `OrganisationTerminatedV2` vindbaar blijft, en dat het resultaat datum-onafhankelijk is (FR-005b, FR-005c)
- [x] T030 [US3] Test FR-007: met twee verschillende waarden voor `OrafinKeyTypeId` in de configuratie geeft dezelfde data andere zoekresultaten, zonder rebuild

**Checkpoint**: codes volgen de bron; de dynamische Orafin-instelling is aantoonbaar live.

---

## Phase 5: Oplevering en bescherming

- [x] T031 [P] Drift-guard: architectuurtest in `test/OrganisationRegistry.ArchitectureTests/` die vaststelt dat `OrganisationSearchListItemView` minstens de `IEventHandler<>`-verzameling van `OrganisationListItemView` afhandelt (plan D-005)
- [x] T032 [P] Swagger-documentatie van `GET /v1/organisations/overview` nakijken (groep "Scherm APIs: Organisaties", voorbeeldrespons gelijk aan de bestaande lijst)
- [x] T033 [P] Regressiecheck: solution-build groen; SqlServer-integratie 76/76, unit 1180/1180, architectuur 14/14, KboMutations 27/27. `ElasticSearch.Tests` en `Api.IntegrationTests` vereisen de opensearch/api-omgeving en zijn hier niet gelopen (FR-009, SC-006)
- [x] T034 Oplevering: de migratie maakt de vier lege tabellen aan; de API herbouwt de projectie daarna zelf bij het opstarten, omdat ze in de whitelist `Projections:InitialiseWhenEmpty` staat en haar tabellen leeg zijn (`InitialiseEmptyProjectionsExtension`). Een manuele rebuild via `POST /v1/tasks` blijft mogelijk. Aandachtspunt: de opstart wacht op de rebuild, dus de startup-/liveness-probes van de API moeten daar ruimte voor laten
- [ ] T035 Performantiecheck op een realistische dataset (vereist een omgeving met echte data; niet lokaal te doen): het code-filter is een deelmatch (`LIKE '%x%'`) en kan de index niet seeken. Bevestig met een uitvoeringsplan dat een *Index Scan* van `IX_OrganisationSearchCodeList_NormalizedValue_KeyTypeId` volstaat zonder *RID Lookup* (de index is covering via `INCLUDE (OrganisationId)`, migratie `IncludeOrganisationIdInSearchCodeIndex`) en dat de overzichtslijst niet merkbaar langzamer is (SC-005). Zo niet: minimale zoeklengte of trigram-/fulltext-oplossing overwegen

---

## Afhankelijkheden

```text
Phase 1 (T001→T005) ──► Phase 2 (US1) ──► Phase 3 (US2)
                                     └──► Phase 4 (US3)
                                                  └──► Phase 5
```

- T001–T005 zijn strikt sequentieel (entiteit → context → migratie → projectie).
- T008/T009 (normalisatie) blokkeren T013 en T015.
- T011–T013 blokkeren T014–T017.
- T018–T020 kunnen parallel met de backend-tests zodra het endpoint uit T016 bestaat.
- Phase 3 en Phase 4 zijn onderling onafhankelijk.

## Besluiten

- **D-004** (plan.md): beslist — eigen endpoint `GET /v1/organisations/overview` op het nieuwe
  read model; `GET /v1/organisations` blijft onaangeroerd voor de overige consumenten.
- Geen openstaande besluiten meer.
