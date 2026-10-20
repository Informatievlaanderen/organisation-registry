# Implementation Plan: Functietype verwijderen (Deleted markeren)

**Branch**: `feat/or-3412-function-type-removal` | **Date**: 2026-10-20 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/010-function-type-removal/spec.md`

## Summary

Een algemeen beheerder kan een functietype verwijderen. Dat volgt 1-op-1 het bestaande mechanisme van hoedanigheden en sleuteltypes (zie research R-1):

- nieuw event `FunctionTypeRemoved`
- de keuzelijst filtert `!IsRemoved`, tenzij `ShowAll`
- projecties zetten `ScheduledForRemoval` op de koppelingen in SQL-read models
- `SyncRemovedItemsService` stuurt daarna per gemarkeerde rij het verwijdercommando

Opgeruimd worden:
1. `RemoveOrganisationCapacity` (bestaat): organisatie-hoedanigheden met dat functietype, als eerste, want `RemoveFunction` weigert zolang een hoedanigheid verwijst
2. `RemoveOrganisationFunction` (bestaat; wordt idempotent): organisatie-functies van dat type
3. `RemoveBodyMandate` (nieuw, met `BodyMandateRemoved`): orgaan-mandaten van dat functietype, inclusief de aangeduide personen. De orgaanpost blijft.

Net als bij hoedanigheden worden nieuwe koppelingen met een verwijderd functietype niet domeinmatig geweigerd.

## Technical Context

**Language/Version**: C# / .NET 8, nullable reference types
**Primary Dependencies**: ASP.NET Core, Be.Vlaanderen.Basisregisters.AggregateSource, FluentValidation, EF Core
**Storage**: SQL Server (event store + read models via EF Core-migraties); ElasticSearch-projecties (organisaties, personen, organen)
**Testing**: xUnit, AggregateSource.Testing (`Specification<>`-tests), API-integratietests (`OrganisationRegistry.Api.IntegrationTests`)
**Target Platform**: Linux containers (API + projectiehosts)
**Project Type**: web-service + Angular admin-UI (`src/OrganisationRegistry.UI`)
**Performance Goals**: geen nieuwe; de opruimprocedure draait periodiek, net als bij hoedanigheden
**Constraints**: append-only events; de opruiming moet idempotent zijn; ze draait als `WellknownUsers.SyncRemovedItemsService` en mag dus niet botsen op KBO- of decentrale beperkingen
**Scale/Scope**: enkele honderden functietypes; per verwijderd type tientallen tot honderden koppelingen

## Constitution Check

| Principe | Status | Toelichting |
|---|---|---|
| I. Event sourcing immutable | ✅ | Alleen nieuwe events (`FunctionTypeRemoved`, `BodyMandateRemoved`); bestaande events blijven ongewijzigd |
| II. Commands → events | ✅ | `RemoveFunctionType`, `RemoveBodyMandate` met handlers en permissiecheck |
| III. CQRS | ✅ | Markering via projecties (`ScheduledForRemoval`); de opruiming leest read models en stuurt commands, zoals vandaag |
| IV. Aggregate boundaries | ✅ | Geen cross-aggregate cascade in het domein; de cascade loopt asynchroon via de hosted service, zoals bij hoedanigheden |
| V. Tests verplicht | ✅ | Unit-tests per aggregate-methode en handler, en een integratietest voor DELETE en de lijstfilter |

Er zijn geen gate-overtredingen. Een bestaand document wijkt wel af: de constitution vermeldt `paket.dependencies`, terwijl AGENTS.md PackageReference gebruikt. Voor deze feature maakt dat niets uit, want er komen geen nieuwe packages bij.

**Post-design re-check**: ✅. `BodyMandateRemoved` is een nieuw event binnen de `Body`-aggregate.

## Project Structure

### Documentation (this feature)

```text
specs/010-function-type-removal/
├── spec.md
├── plan.md              # dit bestand
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── api.md
├── checklists/requirements.md
└── tasks.md             # /speckit.tasks
```

### Source Code (repository root)

```text
src/OrganisationRegistry/
├── Function/
│   ├── FunctionType.cs                      # + IsRemoved, Remove()
│   ├── FunctionTypeCommandHandlers.cs       # + Handle(RemoveFunctionType)
│   ├── Commands/RemoveFunctionType.cs       # nieuw
│   └── Events/FunctionTypeRemoved.cs        # nieuw
├── Organisation/Organisation.cs             # RemoveFunction idempotent
├── Body/
│   ├── Body.cs                              # + RemoveBodyMandate()
│   ├── Mandate/RemoveBodyMandate.cs (+Handler)  # nieuw
│   └── Events/BodyMandateRemoved.cs         # nieuw

src/OrganisationRegistry.Infrastructure/Authorization/
├── Permission.cs                            # + ParametersFunctionTypesDelete
├── RolePermissionMap.cs / ScopePermissionMap.cs  # zelfde rollen als ParametersCapacitiesDelete
└── (WellknownUsers-permissies)              # SyncRemovedItemsService + CanManageFunctions, BodiesCanManageMandates

src/OrganisationRegistry.Api/
├── Backoffice/Parameters/FunctionType/
│   ├── FunctionTypeCommandController.cs     # + [HttpDelete("{id}")]
│   ├── Requests/RemoveFunctionTypeRequest.cs
│   └── Queries/FunctionTypeListQuery.cs     # filter !IsRemoved, ShowAll, IsRemoved in resultaat
└── HostedServices/SyncRemovedItemsService.cs  # volgorde: capacities → functions → body mandates

src/OrganisationRegistry.SqlServer/
├── FunctionType/FunctionTypeListView.cs     # + IsRemoved, handler FunctionTypeRemoved
├── Organisation/OrganisationFunctionListItemView.cs   # + ScheduledForRemoval (IRemovable)
├── Organisation/OrganisationCapacityListItemView.cs   # handler FunctionTypeRemoved → ScheduledForRemoval
├── Body/BodyMandateListItemView.cs          # + ScheduledForRemoval (IRemovable), BodyMandateRemoved
├── DelegationAssignments/DelegationAssignmentListItemView.cs  # BodyMandateRemoved
├── Body/ScheduledActions/PeopleAssignedToBodyMandates/*       # BodyMandateRemoved
└── Migrations/<ts>_FunctionTypeRemoval.cs   # 3 kolommen

src/OrganisationRegistry.ElasticSearch.Projections/
├── Body/BodyHandler.cs, People/Handlers/PersonMandate.cs  # BodyMandateRemoved

src/OrganisationRegistry.Projections.Delegations/Projections/   # DelegationList, PersonMandateList: BodyMandateRemoved
src/OrganisationRegistry.Projections.Reporting/Projections/BodySeatGenderRatioProjection.cs  # BodyMandateRemoved

src/OrganisationRegistry.UI/app/
├── services/functions/*                     # isRemoved, showAll, delete()
└── administration/functions/*               # verwijderknop, filter "toon alles", rij gemarkeerd

test/OrganisationRegistry.UnitTests/          # FunctionType, Body.RemoveBodyMandate, RemoveFunction idempotent, SyncRemovedItemsService-user
test/OrganisationRegistry.Api.IntegrationTests/  # DELETE + lijstfilter
```

**Structure Decision**: bestaande monorepo-structuur. De opzet spiegelt 1-op-1 de hoedanigheden-implementatie (`Capacity/*`, `CapacityListQuery`, `OrganisationCapacityListItemView`, `SyncRemovedItemsService`, `administration/capacities`).

## Complexity Tracking

| Afwijking | Waarom nodig | Eenvoudiger alternatief afgewezen omdat |
|---|---|---|
| Nieuw `BodyMandateRemoved` + handlers in ±8 projecties | Er bestaat geen verwijdering voor orgaan-mandaten | Mandaat beëindigen houdt historiek bij, terwijl het ticket verwijderen vraagt (Q2) |
| `RemoveFunction` idempotent maken | Een exceptie stopt `SyncRemovedItemsService` tot de volgende herstart | Een check in de service is mogelijk, maar wijkt af van `RemoveOrganisationCapacity` |
