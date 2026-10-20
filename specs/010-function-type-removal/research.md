# Research: Functietype verwijderen

## R-1 Bestaand mechanisme (hoedanigheden OR-931 `6834af88b`, sleuteltypes OR-894 `021c3a0e3`)

1. **Parameter verwijderen**: `Capacity.Remove()` is idempotent en geeft `CapacityRemoved`. De handler controleert `Permission.ParametersCapacitiesDelete`, het endpoint is `[HttpDelete("{id}")]`.
2. **Keuzelijst**: `CapacityListView` heeft `IsRemoved`. `CapacityListQuery` filtert `!IsRemoved`, tenzij `ShowAll`.
3. **Geen domeinblokkade**: `AddOrganisationCapacityCommandHandler` controleert `IsRemoved` niet. Alleen de keuzelijst filtert.
4. **Markering in het SQL-read model**: `OrganisationCapacityListItemView.Handle(CapacityRemoved)` en `OrganisationKeyListItemView.Handle(KeyTypeRemoved)` zetten `ScheduledForRemoval = true` (`IRemovable`). Er is geen markering in het domein of in ES, en de detailqueries filteren er niet op: gemarkeerde rijen blijven zichtbaar tot de opruiming.
5. **Opruiming**: `SyncRemovedItemsService` stuurt per gemarkeerde rij `RemoveOrganisationCapacity` of `RemoveOrganisationKey`, als `WellknownUsers.SyncRemovedItemsService`. De interval is standaard 3600s.
6. **Effectieve verwijdering**: het bestaande `…Removed`-event. De bestaande projecties (SQL, ES, Person) verwijderen de rij.

**Decision**: deze zes stappen 1-op-1 volgen voor functietypes.

## R-2 Koppelingen met een functietype

| Koppeling | Aggregate | Read model voor markering | Verwijdercommando |
|---|---|---|---|
| Organisatie-functie | Organisation | `OrganisationFunctionListItemView` (`FunctionId`), heeft vandaag geen `ScheduledForRemoval` | `RemoveOrganisationFunction` bestaat |
| Organisatie-hoedanigheid met functietype | Organisation | `OrganisationCapacityListItemView` (`FunctionId`), heeft al `ScheduledForRemoval` | `RemoveOrganisationCapacity` bestaat |
| Orgaan-mandaat type functietype | Body | `BodyMandateListItemView` (`BodyMandateType.FunctionType`, `DelegatedId == FunctionTypeId`, r.237-251), heeft vandaag geen `ScheduledForRemoval` | **bestaat niet** |

**Orgaanposten vallen buiten scope.** `BodySeat` heeft geen functietype. `AssignFunctionTypeToBodySeat` maakt een `FunctionTypeBodyMandate` op de post aan (`Body.cs:1264`).

## R-3 Orgaan-mandaat verwijderen (nieuw)

- **Decision**: nieuw command `RemoveBodyMandate(BodyId, BodySeatId, BodyMandateId)`, nieuwe methode `Body.RemoveBodyMandate()` en nieuw event `BodyMandateRemoved(BodyId, BodySeatId, BodyMandateId)`.
  - `Apply` roept `bodySeat.UnassignMandate(bodyMandateId)` aan. Die methode bestaat al (`BodySeat.cs:58`).
  - Idempotent, zoals `RemoveOrganisationCapacity`: als het mandaat niet (meer) bestaat, gebeurt er niets.
  - De delegaties (personen aangeduid op het mandaat) horen bij het mandaat en verdwijnen mee. Er komen geen aparte `PersonAssignedToDelegationRemoved`-events; de projecties ruimen op basis van `BodyMandateId` op.
- **Projecties die `BodyMandateRemoved` moeten afhandelen**: alle projecties die op `AssignedFunctionTypeToBodySeat` of `PersonAssignedToDelegation` reageren.
  - SQL: `Body/BodyMandateListItemView` (+ `ScheduledForRemoval`), `DelegationAssignments/DelegationAssignmentListItemView`, `Body/ScheduledActions/PeopleAssignedToBodyMandates/{Active,Future}PeopleAssignedToBodyMandates`
  - Delegations-host: `DelegationListProjection`, `PersonMandateListProjection`
  - Reporting-host: `BodySeatGenderRatioProjection`
  - ES: `Body/BodyHandler`, `People/Handlers/PersonMandate`
- **Alternatives**: het mandaat ongeldig zetten via `ReassignFunctionTypeToBodySeat` met een einddatum. Afgewezen, want het ticket vraagt verwijderen en dat zou ook historiek laten bestaan (Q2).

## R-4 Markering

- `FunctionTypeRemoved` (nieuw event) wordt afgehandeld door:
  - `FunctionTypeListView`: `IsRemoved = true`
  - `OrganisationFunctionListItemView`: `ScheduledForRemoval` waar `FunctionId == id`
  - `OrganisationCapacityListItemView`: `ScheduledForRemoval` waar `FunctionId == id`
  - `BodyMandateListItemView`: `ScheduledForRemoval` waar `BodyMandateType == FunctionType && DelegatedId == id`
- Q2 (ook historiek) is automatisch voldaan, want de markering kijkt niet naar geldigheid.

## R-5 Opruiming in `SyncRemovedItemsService`

- **Volgorde**: eerst hoedanigheden, dan functies. `Organisation.RemoveFunction` (`Organisation.cs:3101`) gooit `FunctionStillUsedInCapacity` zolang een hoedanigheid met dat functietype bestaat. Mandaten zijn onafhankelijk (andere aggregate).
- **Idempotentie**: `RemoveFunction` gooit `OrganisationFunctionNotFound` als de functie al weg is, en een exceptie stopt de hosted service tot de volgende herstart (`BackgroundService.ExecuteAsync` logt en stopt). De handler moet daarom stil niets doen bij een onbekende functie, net zoals `RemoveOrganisationCapacity`. Geverifieerd: `OrganisationFunctionNotFound` wordt enkel in `Organisation.RemoveFunction` gegooid en in één unit-test verwacht (`WhenRemovingAnOrganistaionFunction.cs:76`). Die test moet aangepast worden.
- **Autorisatie**: `RemoveOrganisationFunction` gebruikt `FunctionPolicy` (`CanManageFunctions`) en `RemoveBodyMandate` gebruikt `BodyPolicy` (`BodiesCanManageMandates`). `WellknownUsers.SyncRemovedItemsService` moet die permissies krijgen, met tests zoals `WhenRemovingAnOrganisationCapacityAsSyncRemovedItemsService`.

## R-6 Nieuwe koppelingen en keuzelijsten

- **Decision**: geen domeinblokkade, zoals bij hoedanigheden (R-1.3).
- Keuzelijsten met functietypes (geverifieerd):
  - `FunctionService.getAllFunctions()`: `organisation/functions/create` en `update`
  - `FunctionResolver`: orgaan-mandaat `linkfunction` en `updatefunction` (`body/detail/detail-routing.module.ts:301,323`)
  - Beide roepen `GET /v1/functiontypes` op zonder `showAll`, dus de backendfilter volstaat. Net als bij hoedanigheden is geen frontend-aanpassing nodig voor de keuzelijsten.
- De keuzelijst "functie" bij een organisatie-hoedanigheid toont de functies van een persoon (`PersonFunctionService`), niet de functietypes. Die verdwijnen vanzelf zodra de organisatie-functies opgeruimd zijn.

## R-7 Autorisatie verwijderen

- **Decision**: een nieuwe `Permission.ParametersFunctionTypesDelete`, voor dezelfde rollen en scopes als `ParametersCapacitiesDelete`.
- Plaatsen: `Permission.cs:248`, `RolePermissionMap.cs:79,220`, `ScopePermissionMap.cs:94`. Tests: `VoMedewerkerRoleTests.cs:108` (InlineData) en de integratie-permissiematrix `Security/PermissionMatrix/Parameters/Given_Algemeenbeheerder.cs:54` en `Given_Roles_Without_Parameter_Permissions.cs:77` (capacity-delete als voorbeeld).

## R-8 Event-naam

- **Decision**: `FunctionTypeRemoved`. Dat is duidelijker dan `FunctionRemoved`, dat verward kan worden met `OrganisationFunctionRemoved`.
