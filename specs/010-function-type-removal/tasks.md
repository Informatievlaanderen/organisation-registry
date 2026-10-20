---
description: "Task list — OR-3412 functietype verwijderen"
---

# Tasks: Functietype verwijderen (Deleted markeren)

**Input**: `specs/010-function-type-removal/` (plan.md, spec.md, research.md, data-model.md, contracts/api.md, quickstart.md)
**Tests**: verplicht volgens constitution V. Voor elk aggregate- en handlergedrag schrijven we eerst een `Specification<>`-test.
**Referentie**: OR-931 hoedanigheden (`git show 6834af88b`), OR-894 sleuteltypes (`021c3a0e3`).

## Format: `[ID] [P?] [Story] Beschrijving`
- **[P]**: kan parallel lopen (ander bestand, geen open afhankelijkheid)

---

## Phase 1: Setup

- [ ] T001 Controleer dat de baseline groen is: `dotnet build` en `dotnet test test/OrganisationRegistry.UnitTests` op branch `feat/or-3412-function-type-removal`

---

## Phase 2: Foundational (blokkeert alle stories)

- [ ] T002 Voeg `ParametersFunctionTypesDelete` toe na `ParametersFunctionTypesWrite` in `src/OrganisationRegistry.Infrastructure/Authorization/Permission.cs`
- [ ] T003 Ken `ParametersFunctionTypesDelete` toe aan dezelfde rollen als `ParametersCapacitiesDelete` in `src/OrganisationRegistry.Infrastructure/Authorization/RolePermissionMap.cs` (r.79 en r.220), en aan dezelfde scope in `src/OrganisationRegistry.Infrastructure/Authorization/ScopePermissionMap.cs` (r.94)
- [ ] T004 [P] Voeg `[InlineData(Permission.ParametersFunctionTypesDelete)]` toe in `test/OrganisationRegistry.UnitTests/Authorization/VoMedewerkerRoleTests.cs` (naast r.108) en laat `RolePermissionMapTests` en `ScopePermissionMapTests` slagen
- [ ] T005 Maak het event `FunctionTypeRemoved(Guid functionTypeId)` in `src/OrganisationRegistry/Function/Events/FunctionTypeRemoved.cs`, naar het model van `src/OrganisationRegistry/Capacity/Events/CapacityRemoved.cs`

**Checkpoint**: de permissie en het event bestaan en de build is groen.

---

## Phase 3: User Story 1, functietype als verwijderd markeren (P1) 🎯 MVP

**Goal**: een algemeen beheerder kan een functietype verwijderen, en dat gebeurt idempotent.
**Independent Test**: `DELETE /v1/functiontypes/{id}` geeft 204 en `FunctionTypeRemoved`; een tweede keer volgt geen event; zonder permissie volgt een weigering.

### Tests (eerst, moeten falen)
- [ ] T006 [P] [US1] Unit-test `test/OrganisationRegistry.UnitTests/FunctionTypes/RemoveFunctionType/WhenRemovingAFunctionType.cs`: na `FunctionCreated` levert `RemoveFunctionType` `FunctionTypeRemoved` op (structuur van `test/OrganisationRegistry.UnitTests/KeyTypes/RemoveKeyType`)
- [ ] T007 [P] [US1] Unit-test `.../RemoveFunctionType/WhenRemovingARemovedFunctionType.cs`: na `FunctionTypeRemoved` volgt geen event
- [ ] T008 [P] [US1] Unit-test `.../RemoveFunctionType/WhenRemovingAFunctionTypeWithoutPermission.cs`: zonder `ParametersFunctionTypesDelete` volgt een weigering
- [ ] T009 [P] [US1] Integratietests, naar het model van capacity-delete: in `test/OrganisationRegistry.Api.IntegrationTests/Security/PermissionMatrix/Parameters/Given_Algemeenbeheerder.cs` (zie r.54) verwacht `DELETE /v1/functiontypes/{id}` 204, en in `Given_Roles_Without_Parameter_Permissions.cs` (zie r.77) 403

### Implementation
- [ ] T010 [US1] Maak het command `RemoveFunctionType(FunctionTypeId)` in `src/OrganisationRegistry/Function/Commands/RemoveFunctionType.cs` (model: `Capacity/Commands/RemoveCapacity.cs`)
- [ ] T011 [US1] Voeg aan `src/OrganisationRegistry/Function/FunctionType.cs` toe: `IsRemoved`, `Remove()` (`if (IsRemoved) return; ApplyChange(new FunctionTypeRemoved(Id))`) en `Apply(FunctionTypeRemoved)`
- [ ] T012 [US1] Voeg `Handle(RemoveFunctionType)` met `.RequiresPermission(Permission.ParametersFunctionTypesDelete)` toe aan `src/OrganisationRegistry/Function/FunctionTypeCommandHandlers.cs`, en registreer de interface (model: `CapacityCommandHandlers.cs:65`)
- [ ] T013 [US1] Maak `RemoveFunctionTypeRequest` met mapping in `src/OrganisationRegistry.Api/Backoffice/Parameters/FunctionType/Requests/RemoveFunctionTypeRequest.cs` (model: `Capacity/Requests/RemoveCapacityRequest.cs`)
- [ ] T014 [US1] Voeg `[HttpDelete("{id}")]` met `RequiredPermissions = [Permission.ParametersFunctionTypesDelete]` toe aan `src/OrganisationRegistry.Api/Backoffice/Parameters/FunctionType/FunctionTypeCommandController.cs` (model: `CapacityCommandController.cs:61-75`)
- [ ] T015 [P] [US1] Voeg in de UI `isRemoved` toe aan `function.model.ts` en `function-list-item.model.ts`, en een methode `delete(id)` in `function.service.ts`, allemaal in `src/OrganisationRegistry.UI/app/services/functions/` (model: `services/capacities/capacity.service.ts`)
- [ ] T016 [US1] Voeg in de UI een verwijderknop toe, plus de klasse `data-table__body-history` voor rijen met `isRemoved`, in `src/OrganisationRegistry.UI/app/administration/functions/components/list/list.{component.ts,template.html,style.css}` (model: `administration/capacities/components/list`)
- [ ] T017 [US1] Laat de UI-detailpagina een verwijderd functietype tonen en niet laten bewerken, in `src/OrganisationRegistry.UI/app/administration/functions/detail/detail.component.ts`, en verwerk de delete in `overview/overview.component.ts` (model: commit `6834af88b`, `administration/capacities/{detail,overview}`)

**Checkpoint**: US1 werkt op zichzelf (T006-T009 zijn groen).

---

## Phase 4: User Story 2, keuzelijsten zonder verwijderde functietypes (P1)

**Goal**: `GET /v1/functiontypes` geeft standaard enkel actieve types; `showAll` geeft alle types met `isRemoved`.
**Independent Test**: na `FunctionTypeRemoved` ontbreekt het type in de keuzelijst bij "functie toevoegen" en bij "orgaan-mandaat → functietype koppelen"; in het beheerscherm staat het er wel, gemarkeerd.

### Tests
- [ ] T018 [P] [US2] Integratietest in `test/OrganisationRegistry.Api.IntegrationTests`: na een delete geeft `GET /v1/functiontypes` het type niet terug, en met `x-filtering: {"showAll":true}` wel, met `isRemoved: true`

### Implementation
- [ ] T019 [US2] Voeg `IsRemoved` (property, EF-config, default false) en `IEventHandler<FunctionTypeRemoved>` toe aan `src/OrganisationRegistry.SqlServer/FunctionType/FunctionTypeListView.cs` (model: `Capacity/CapacityListView.cs`)
- [ ] T020 [US2] Breid `src/OrganisationRegistry.Api/Backoffice/Parameters/FunctionType/Queries/FunctionTypeListQuery.cs` uit: een filterklasse met `ShowAll`, de filter `!IsRemoved` en `IsRemoved` in het resultaat (model: `CapacityListQuery.cs:40-75`)
- [ ] T021 [US2] Laat `src/OrganisationRegistry.Api/Backoffice/Parameters/FunctionType/FunctionTypeController.cs` `ExtractFilteringRequest<FunctionTypeListQuery.FunctionTypeListFilter>()` gebruiken
- [ ] T022 [P] [US2] Voeg in de UI `showAll` toe aan `src/OrganisationRegistry.UI/app/services/functions/function-filter.model.ts`, zet in de beheer-overview `filter.showAll = true`, en voeg een checkbox toe in `administration/functions/components/filter` (model: `administration/capacities/overview/overview.component.ts:53` en `components/filter/filter.component.ts:27`)
- [ ] T023 [US2] Controleer dat `getAllFunctions()` (organisation/functions create/update) en `FunctionResolver` (body/mandates linkfunction/updatefunction) geen `showAll` meesturen. Daarvoor is geen codewijziging nodig.

**Checkpoint**: US1 en US2 werken samen (dit is de MVP).

---

## Phase 5: User Story 3, bestaande koppelingen opruimen (P2)

**Goal**: na `FunctionTypeRemoved` markeren projecties de koppelingen met `ScheduledForRemoval`, en `SyncRemovedItemsService` ruimt ze op in deze volgorde: hoedanigheden, functies, mandaten.
**Independent Test**: quickstart.md, manueel scenario.

### 5a. Organisatie-functie idempotent en toegelaten voor de systeemgebruiker
- [ ] T024 [US3] Pas `test/OrganisationRegistry.UnitTests/Organisation/RemoveOrganisationFunction/WhenRemovingAnOrganistaionFunction.cs:76` aan: bij een onbekende functie volgt geen event in plaats van `OrganisationFunctionNotFound`
- [ ] T025 [P] [US3] Maak de test `test/OrganisationRegistry.UnitTests/Organisation/RemoveOrganisationFunction/WhenRemovingAnOrganisationFunctionAsSyncRemovedItemsService.cs` (model: `RemoveOrganisationCapacity/WhenRemovingAnOrganisationCapacityAsSyncRemovedItemsService.cs`). Neem hierin de KBO/decentrale-casus uit de edge cases op (research A1: `FunctionPolicy` heeft geen KBO-check, dus dit moet slechts één keer geverifieerd worden).
- [ ] T026 [US3] Laat `Organisation.RemoveFunction` (`src/OrganisationRegistry/Organisation/Organisation.cs:3095`) `return` geven in plaats van `throw new OrganisationFunctionNotFound()`. Verwijder `OrganisationFunctionNotFound.cs` als het nergens anders meer gebruikt wordt.
- [ ] T027 [US3] Voeg `Permission.CanManageFunctions` en `Permission.BodiesCanManageMandates` toe aan `WellknownUsers.SyncRemovedItemsService` in `src/OrganisationRegistry.Infrastructure/WellknownUsers.cs:18-22`, en werk de commentaar bij

### 5b. Orgaan-mandaat verwijderen (nieuw domeinconcept)
- [ ] T028 [P] [US3] Unit-tests in `test/OrganisationRegistry.UnitTests/Body/WhenRemovingABodyMandate/`:
  - (a) een functietype-mandaat levert `BodyMandateRemoved` op
  - (b) een onbekend mandaat levert geen event op
  - (c) na verwijdering faalt `AssignPersonToDelegation` op dat mandaat, maar de post bestaat nog
  - (d) als `SyncRemovedItemsService` is het toegelaten
- [ ] T029 [US3] Maak het event `BodyMandateRemoved(Guid bodyId, Guid bodySeatId, Guid bodyMandateId)` in `src/OrganisationRegistry/Body/Mandate/Events/BodyMandateRemoved.cs`
- [ ] T030 [US3] Maak het command `RemoveBodyMandate` en `RemoveBodyMandateCommandHandler` (`.WithBodyPolicy(Permission.BodiesCanManageMandates)`) in `src/OrganisationRegistry/Body/Mandate/` (model: `Body/Seat/AssignFunctionTypeToBodySeatCommandHandler.cs`), en registreer de handler
- [ ] T031 [US3] Voeg in `src/OrganisationRegistry/Body/Body.cs` `RemoveBodyMandate(BodySeatId, BodyMandateId)` toe (idempotent) en `Apply(BodyMandateRemoved)`, dat `bodySeat.UnassignMandate(id)` aanroept

### 5c. Markering (SQL-projecties + migratie)
- [ ] T032 [US3] Voeg `ScheduledForRemoval` toe aan `OrganisationFunctionListItem` (`IRemovable`) in `src/OrganisationRegistry.SqlServer/Organisation/OrganisationFunctionListItemView.cs`, met EF-config en `IEventHandler<FunctionTypeRemoved>`: markeer waar `FunctionId == id` (model: `OrganisationCapacityListItemView.cs:136-146`)
- [ ] T033 [US3] Voeg `IEventHandler<FunctionTypeRemoved>` toe aan `src/OrganisationRegistry.SqlServer/Organisation/OrganisationCapacityListItemView.cs`: markeer waar `FunctionId == id`
- [ ] T034 [US3] Voeg `ScheduledForRemoval` toe aan `BodyMandateListItem` (`IRemovable`) in `src/OrganisationRegistry.SqlServer/Body/BodyMandateListItemView.cs`, met EF-config:
  - `IEventHandler<FunctionTypeRemoved>` markeert waar `BodyMandateType == FunctionType && DelegatedId == id`
  - `IEventHandler<BodyMandateRemoved>` verwijdert de rij
- [ ] T035 [US3] Maak de EF-migratie `AlterTables_FunctionTypeRemoval` (3 kolommen, default false): `cd src/OrganisationRegistry.SqlServer && dotnet ef migrations add AlterTables_FunctionTypeRemoval --context OrganisationRegistryContext --startup-project ../OrganisationRegistry.Api/`

### 5d. Opruiming
- [ ] T036 [US3] Breid `src/OrganisationRegistry.Api/HostedServices/SyncRemovedItemsService.cs` uit. Hernoem `RemoveOrganisationItems<TItem, TCommand>` naar één generieke `RemoveItems<TItem, TCommand>` (geen organisatie-gebonden naam, `BodyMandateListItem` valt er ook onder) en voeg na de capacities deze twee calls toe, in deze gegarandeerde volgorde (research R-5): `OrganisationFunctionList` → `RemoveOrganisationFunction`, daarna `BodyMandateList` → `RemoveBodyMandate`.

### 5e. Projecties voor `BodyMandateRemoved` (verwijder rijen op `BodyMandateId`)
- [ ] T037 [P] [US3] `src/OrganisationRegistry.SqlServer/DelegationAssignments/DelegationAssignmentListItemView.cs`
- [ ] T038 [P] [US3] `src/OrganisationRegistry.SqlServer/Body/ScheduledActions/PeopleAssignedToBodyMandates/ActivePeopleAssignedToBodyMandates.cs` en `FuturePeopleAssignedToBodyMandates.cs`
- [ ] T039 [P] [US3] `src/OrganisationRegistry.Projections.Delegations/Projections/DelegationListProjection.cs`
- [ ] T040 [P] [US3] `src/OrganisationRegistry.Projections.Delegations/Projections/PersonMandateListProjection.cs`
- [ ] T041 [P] [US3] `src/OrganisationRegistry.Projections.Reporting/Projections/BodySeatGenderRatioProjection.cs`: verwijder bij `BodyMandateRemoved` de bijdrage van dat mandaat (en van de personen die erop aangeduid waren) uit de geslachtsverhouding van de post en van het orgaan; de tellingen van de post zelf blijven ongewijzigd (de post bestaat nog).
- [ ] T042 [P] [US3] ES: `src/OrganisationRegistry.ElasticSearch.Projections/Body/BodyHandler.cs` (mandaat uit het orgaandocument)
- [ ] T043 [P] [US3] ES: `src/OrganisationRegistry.ElasticSearch.Projections/People/Handlers/PersonMandate.cs` (mandaat uit de persoonsdocumenten)
- [ ] T044 [US3] Sluit twee gaten af met een brede grep: (a) of er nog registraties/routes zijn waar `BodyMandateRemoved` aan moet worden toegevoegd (grep op de registraties van `AssignedFunctionTypeToBodySeat`); (b) of read models die `OrganisationFunctionRemoved`/`OrganisationCapacityRemoved` afhandelen ook bereikt worden (grep `OrganisationFunctionRemoved` en `OrganisationCapacityRemoved` over heel `src`, inclusief `OrganisationRegistry.Projections.*`; verwacht resultaat: `OrganisationFunctionListItemView`, `PersonFunctionListItemView`, ES `OrganisationFunction`/`PersonFunction`, `OrganisationCapacityListItemView`, `PersonCapacityListItemView`, ES `OrganisationCapacity`). Ontbreekt er een, voeg dan een taak toe.

- [ ] T044b [US3] Integratietest (constitution V) in `test/OrganisationRegistry.Api.IntegrationTests`: verwijder een functietype, en asserteer daarna in de test-database dat de drie koppelingen `ScheduledForRemoval` hebben (model: de integratietests met de `Tests.Shared`-harness).

**Checkpoint**: het volledige quickstart-scenario werkt.

---

## Phase 6: Polish

- [ ] T045 Volledige build en tests: `dotnet build && dotnet test`
- [ ] T046 [P] UI: `npm run lint` en `npm run build` in `src/OrganisationRegistry.UI`
- [ ] T047 Loop het manuele scenario uit `quickstart.md` door met `SyncRemovedItemsService` aan (10s in development)
- [ ] T048 Commits: één per fase, als conventional commit (`feat: OR-3412 ...`)

---

## Dependencies

- Setup → Foundational → US1 → US2 → US3.
- US2 hangt alleen van Foundational en T011 af (het event), dus het kan parallel met de UI-taken van US1.
- In US3:
  - 5a en 5b zijn onafhankelijk van elkaar
  - 5c vraagt T029 (het event)
  - 5d vraagt 5a, 5b en 5c
  - 5e vraagt T029

## Parallel examples

- US1: T006, T007, T008 en T009 tegelijk; T015 parallel met T010-T014.
- US3: T037-T043 allemaal parallel na T029.

## Implementation strategy

1. MVP = Phase 1-4: verwijderen en de gefilterde keuzelijst. Dat is apart oplevervaardig, zoals bij hoedanigheden.
2. Daarna US3 met de opruiming. 5b en 5e vormen het grootste blok, want verwijderen van orgaan-mandaten is nieuw.
