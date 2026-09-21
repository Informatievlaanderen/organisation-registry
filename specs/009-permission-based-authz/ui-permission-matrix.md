# UI Permission Matrix — Target State

**Feature:** 009-permission-based-authz
**Status:** Provided by product/user (2026-09-02) — Keys row implemented in commit `c3b0d4af5` (Model C). Overige resources: DEFERRED (zelfde pattern).
**Purpose:** Canonical target-state per role/resource. Drives `RolePermissionMap` en restriction design.

**Legenda:**
- `R` = Read
- `CRUD` = Create/Read/Update/Delete
- `CRU` = geen delete
- `*` = restrictie van toepassing (id-whitelist of org-scope live via SecurityService)
- `–` = geen toegang / niet van toepassing

## Matrix

| Scherm/functionaliteit | UI-recht op organisatierespons | Publieke rol | VO medewerker | Algemeen beheerder | Decentraal beheerder | Vlimpers beheerder | Orgaan beheerder | Regelgeving / Deugdelijk bestuur beheerder |
|---|---|---|---|---|---|---|---|---|
| Organisatie | `canEdit`, `canDelete`, `organisations:create` (globaal recht) | R | R | CRUD | CRU | CRUD | R | R |
| Dochters | `canManageChildren` | R | R | CRUD | R | CRUD | R | R |
| Contacten | `canManageContacts` | R | R | CRUD | R | R | R | R |
| Bankrekeningnummers | – | – | – | – | – | – | – | – |
| Functies | `canViewFunctions`, `canManageFunctions` | – | R | CRUD | CRUD | R | R | R |
| Hoedanigheden | `canViewCapacities`, `canManageCapacities` | – | R | CRUD | CRUD* | R* | R | CRUD* |
| Locaties | `canManageLocations` | R | R | CRUD | CRUD | R | R | R |
| Gebouwen | `canManageBuildings` | R | R | CRUD | CRUD | R | R | R |
| Historiek | – | R | R | CRUD | R | CRUD | R | R |
| Benamingen | `canManageLabels` | R | R | CRUD | CRUD* | CRUD* | R | R |
| Classificaties | `canManageClassifications` | R | R | CRUD | CRUD | R | R | CRUD* |
| Toepassingsgebieden | `canManageFormalFrameworks` | R | R | CRUD | CRUD* | CRUD* | R | CRUD* |
| Sleutels | `canManageKeys` | R | R | CRUD | R | CRUD* | R | R |
| Regelgeving | `canManageRegulations` | R | R | CRUD | R | – | R | CRUD |
| Organen | `canManageBodies` | R | R | CRUD | CRUD | R | R | R |
| Relaties | `canManageRelations` | R | R | CRUD | CRUD | R | R | R |
| Openingsuren *(gaat eruit)* | – | – | – | – | – | – | – | – |
| KBO-koppeling | `canViewKbo`, `canManageKbo` | – | – | CRUD | – | – | – | R |
| Vlimpers | `canViewVlimpers`, `canManageVlimpers` | – | – | CRUD | – | – | – | R |

## Personen (los van organisatie/orgaan-scope)

**Status:** Geïmplementeerd. `Persoon` is geen organisatiescherm (geen `canEdit`/organisatie-scope);
onderstaande rechten gelden globaal, ongeacht welke organisatie of orgaan de persoon (mede)
vertegenwoordigt.

| Scherm/functionaliteit | Permissie | Publieke rol | VO medewerker | Algemeen beheerder | Decentraal beheerder | Vlimpers beheerder | Orgaan beheerder | Regelgeving / Deugdelijk bestuur beheerder |
|---|---|---|---|---|---|---|---|---|
| Persoon | `PeopleWrite` (create/update, geen delete) | R | R | CRU | R | R | R | R |
| Functies | `PeopleFunctionsRead` | – | R | R | R | R | R | R |
| Hoedanigheden | `PeopleCapacitiesRead` | – | R | R | R | R | R | R |
| Mandaten | – (geen permissie vereist) | R | R | R | R | R | R | R |

- `Persoon` lezen (`GET /v1/people`, `GET /v1/people/{id}`) vereist geen permissie: dit is publiek
  toegankelijk, ook zonder token.
- `Persoon` aanmaken/aanpassen (`POST`/`PUT /v1/people`) vereist `PeopleWrite`, enkel toegekend aan
  AlgemeenBeheerder (en Developer). Alle andere rollen krijgen 403. Er is geen delete-permissie:
  personen kunnen nooit verwijderd worden via de API.
- `Functies`/`Hoedanigheden` lezen vereist resp. `PeopleFunctionsRead`/`PeopleCapacitiesRead`,
  toegekend aan elke backoffice-rol (AlgemeenBeheerder, DecentraalBeheerder, VlimpersBeheerder,
  OrgaanBeheerder, RegelgevingBeheerder). Een niet-ingelogde ("Publiek") aanvraag krijgt 401
  (geen geldig token) — er bestaat geen echte "Publiek"/"VO medewerker"-rol in het systeem, dus
  deze kolommen zijn hier documentair (elke ingelogde backoffice-gebruiker = "VO medewerker").
- `Mandaten` lezen vereist geen permissie: publiek toegankelijk zoals `Persoon` lezen.
- Er bestaat geen write-permissie voor Functies/Hoedanigheden op het Personen-scherm: die worden
  uitsluitend beheerd vanaf de organisatiekant (`CanManageFunctions`/`CanManageCapacities`).

## Systeem (los van organisatie/orgaan-scope)

**Status:** Geïmplementeerd. `Systeem` is geen organisatiescherm; het gaat om drie
technische/admin-schermen (Statistieken, Events, Stopgezet in KBO). Er is bewust **geen
fijnmazig recht per scherm** — één ongesplitst globaal recht (`Permission.System`, `/v1/me`
string `system:read`) dekt alle drie.

| Scherm/functionaliteit | Permissie | Publieke rol | VO medewerker | Algemeen beheerder | Decentraal beheerder | Vlimpers beheerder | Orgaan beheerder | Regelgeving / Deugdelijk bestuur beheerder |
|---|---|---|---|---|---|---|---|---|
| Statistieken | `System` | – | – | R | – | – | – | – |
| Events | `System` | – | – | R | – | – | – | – |
| Stopgezet in KBO | `System` | – | – | R | – | – | – | – |

- Enkel `AlgemeenBeheerder` (en `Developer`) krijgt `Permission.System`. Alle andere rollen
  (inclusief `CjmBeheerder`/Orafin) krijgen 403 op `GET /v1/events` en
  `GET /v1/organisations/kbo/terminated`.
- `CjmBeheerder` had voorheen (rol-gebaseerd) toegang tot `kbo/terminated` —
  dat is met de overstap naar het ongesplitste `System`-recht komen te vervallen.
- `Events` (`GET /v1/events`) gebruikte voorheen een ongebruikt `CanReadEvents`-recht dat aan
  geen enkele rol was toegekend (dus 403 voor iedereen, ook AlgemeenBeheerder) — dit was een
  latente bug, nu gefixt via `Permission.System`.
- **Statistieken heeft geen backend-endpoint** (de legacy Angular-UI roept `/v1/status/stats`
  aan, maar er bestaat geen bijhorende controller). Er is dus niets te gaten op serverniveau;
  dit blijft een gedocumenteerde gap tot het endpoint (opnieuw) geïmplementeerd wordt.
- Read-only: er is geen create/update/delete op één van de drie schermen.

## Importeren (los van organisatie/orgaan-scope)

**Status:** Geïmplementeerd. `Importeren` is geen organisatiescherm; één globaal recht
(`Permission.CanImport`, `/v1/me`-string `imports`) dekt het volledige scherm.

| Scherm/functionaliteit | Permissie | Publieke rol | VO medewerker | Algemeen beheerder | Decentraal beheerder | Vlimpers beheerder | Orgaan beheerder | Regelgeving / Deugdelijk bestuur beheerder |
|---|---|---|---|---|---|---|---|---|
| Importeren | `CanImport` | – | – | CRUD | – | CRUD | – | – |

- `AlgemeenBeheerder` en `Developer` krijgen `Permission.CanImport` **onbeperkt** (elke
  organisatie). `VlimpersBeheerder` krijgt `CanImport` enkel als **beperkte grant**,
  geldig per doelorganisatie: die organisatie moet onder Vlimpers-beheer staan
  (`ChildRestrictions.UnderVlimpersManagement`, dezelfde restrictie als `CanManageChildren`).
  Alle andere rollen (DecentraalBeheerder, OrgaanBeheerder, RegelgevingBeheerder, CjmBeheerder,
  Orafin) krijgen 403 op alle import-endpoints, lezen inbegrepen (fail-closed).
- De controller (`ImportOrganisationsController`, route `imports`) is gemigreerd van het
  verouderde rol-gebaseerde `[OrganisationRegistryAuthorize(Role.AlgemeenBeheerder,
  Role.VlimpersBeheerder)]` naar `RequiredPermissions = [Permission.CanImport]`; deze
  controllercheck kent geen organisatiecontext (upload-tijdstip), dus is functioneel
  ongewijzigd t.o.v. voorheen: elke houder van `CanImport` (onbeperkt of beperkt) mag
  uploaden/lezen.
- **Handler-niveau (`ImportPolicy`)**: gemigreerd van handmatige rol-checks (`IsInAnyOf`
  + `VlimpersPolicy`-delegatie) naar het permissie/restrictie-patroon, analoog aan
  `ChildPolicy`. Voor elke doelorganisatie in de import wordt `IsSatisfiedFor(CanImport,
  UserContext, OrganisationContext, VlimpersManagementContext)` geëvalueerd; dit is waar
  de Vlimpers-restrictie voor `VlimpersBeheerder` effectief wordt afgedwongen (per
  organisatie, niet enkel op controllerniveau).
- **Er bestaan momenteel geen Update- of Delete-endpoints** voor imports — enkel Create
  (`POST /v1/imports/organisation-creations`, `POST /v1/imports/organisation-terminations`)
  en Read (`GET /v1/imports`, `GET /v1/imports/{id}/content`). `CanImport` dekt volledige CRUD
  zodra dergelijke endpoints ooit worden toegevoegd; tot dan is dit een gedocumenteerde gap,
  analoog aan de Statistieken-gap bij Systeem.

## Interpretatie per sterretje-cel

| Cel | Betekenis (voorlopig, valideren tijdens implementatie) |
|---|---|
| **DB Hoedanigheden CRUD\*** | Alleen op eigen organisatie (SecurityService live) — geen id-restrictie? |
| **VB Hoedanigheden R\*** | Read-restrictie: alleen VB-relevante capacity-instanties? Nog te verhelderen. |
| **RDB Hoedanigheden CRUD\*** | `CapacityIdsOwnedByRegelgevingDbBeheerder` whitelist. |
| **DB Benamingen CRUD\*** | Alleen op eigen organisatie + welke labeltypes precies? (huidig gedrag: alles behalve `LabelIdsAllowedForVlimpers`). Te verhelderen. |
| **VB Benamingen CRUD\*** | `LabelIdsAllowedForVlimpers` whitelist. |
| **RDB Classificaties CRUD\*** | `OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder` (+ Cjm via CC scope?). |
| **DB Toepassingsgebieden CRUD\*** | Alleen eigen org — id-restrictie te verhelderen. |
| **VB Toepassingsgebieden CRUD\*** | `FormalFrameworkIdsOwnedByVlimpers` whitelist. |
| **RDB Toepassingsgebieden CRUD\*** | `FormalFrameworkIdsOwnedByRegelgevingDbBeheerder` whitelist. |
| **VB Sleutels CRUD\*** | **2-assig (Model C, shipped)**: `KeyIdsAllowedForVlimpers` allowlist **AND** organisatie moet onder Vlimpers-beheer staan. Beide moeten passen. Zie `KeyRestrictions.VlimpersManaged` + `RequireUnderVlimpersManagementRestriction`. |
| **DB Sleutels R** | **Geen `CanManageKeys`** — verandering vs oud gedrag (DB had write op non-Vlimpers/Orafin keytypes). **Beslist en geïmplementeerd** in `c3b0d4af5`. |

## Verandering vs huidig gedrag

De volgende cellen wijken af van de huidige policy-implementaties en vereisen expliciete verificatie:

1. **DB Sleutels: R (was CRUD op subset)** — grote reductie.
2. **DB Contacten: R (was CRUD?)** — te checken.
3. **VB Hoedanigheden: R\* (waarschijnlijk was helemaal geen toegang)** — mogelijk nieuwe capability.
4. **VB Historiek: CRUD** — nieuwe capability?
5. **RDB Regelgeving: CRUD (was: alleen bepaalde rol)** — te checken.

## Deferrals

- Bankrekeningnummers en Openingsuren staan buiten scope (Bankaccounts blijft uit auth-scope; Openingsuren gaat verdwijnen).
- `canViewFunctions`/`canViewCapacities`/`canViewKbo`/`canViewVlimpers` "view"-permissions: apart implementeren als target-state het nodig heeft; nu geen deel van policy-migratie.
- Multi-role users: matrix is per-rol; effectieve permissions = union (zie architectuur-doc).
