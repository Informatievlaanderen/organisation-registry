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
