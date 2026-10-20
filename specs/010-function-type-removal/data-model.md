# Data Model: Functietype verwijderen

## Domein

### FunctionType (uitgebreid)
- `IsRemoved: bool`
- `Remove()`: als `IsRemoved` al waar is, gebeurt er niets; anders `FunctionTypeRemoved`.

### Body (uitgebreid)
- `RemoveBodyMandate(BodySeatId, BodyMandateId)`: als de post of het mandaat niet bestaat, gebeurt er niets; anders `BodyMandateRemoved`. `Apply` roept `bodySeat.UnassignMandate(id)` aan. De post blijft bestaan.

### Organisation (aangepast)
- `RemoveFunction`: idempotent maken als de functie niet gevonden wordt (zie research R-5).

## Events (nieuw)

| Event | Velden |
|---|---|
| `FunctionTypeRemoved` | `FunctionTypeId` |
| `BodyMandateRemoved` | `BodyId`, `BodySeatId`, `BodyMandateId` |

## Commands (nieuw)

| Command | Policy |
|---|---|
| `RemoveFunctionType(FunctionTypeId)` | `ParametersFunctionTypesDelete` |
| `RemoveBodyMandate(BodyId, BodySeatId, BodyMandateId)` | `BodyPolicy(BodiesCanManageMandates)`; wordt gebruikt door `SyncRemovedItemsService`, er is geen API-endpoint |

## Read models (SQL, één migratie)

| Tabel | Wijziging |
|---|---|
| `FunctionTypeList` | + `IsRemoved bit not null default 0` |
| `OrganisationFunctionList` | + `ScheduledForRemoval bit not null default 0`, `IRemovable` |
| `BodyMandateList` | + `ScheduledForRemoval bit not null default 0`, `IRemovable` |
| `OrganisationCapacityList` | geen kolom, wel een extra handler |

## Flow

```
DELETE functietype ─▶ FunctionTypeRemoved
   ├─ FunctionTypeList.IsRemoved = true            (keuzelijst filtert)
   ├─ OrganisationCapacityList.ScheduledForRemoval  (FunctionId)
   ├─ OrganisationFunctionList.ScheduledForRemoval  (FunctionId)
   └─ BodyMandateList.ScheduledForRemoval           (FunctionType, DelegatedId)

SyncRemovedItemsService (elke interval):
   1. RemoveOrganisationCapacity ─▶ OrganisationCapacityRemoved
   2. RemoveOrganisationFunction ─▶ OrganisationFunctionRemoved
   3. RemoveBodyMandate          ─▶ BodyMandateRemoved
```
