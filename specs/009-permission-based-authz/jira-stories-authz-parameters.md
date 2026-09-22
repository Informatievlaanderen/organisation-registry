# Jira-stories — Authz parameters & /v1/me (beknopt)

Code-verificatie @ HEAD `ac598dec3`. Labels: `DevProvided`, `RollenEnOrganisatie`.

---

## 1. OR: [Authz] Rol "VO medewerker" toevoegen
- Enum + RoleMapping + RolePermissionMap + /v1/me-map (alleen leesrechten + hoedanigheden).
- `canEdit` enkel op hoedanigheden.
- Tests: mapping-unit + PermissionMatrix (capacities 200 / rest 403).
- *Decentraal beheerder VO apart: afbakening nog TBD.*

## 2. OR: [Authz] /v1/me — permissieformaat aligneren
- Code geeft `org.organisations:create`, analyse zegt `organisations:create` — prefixen afspreken.
- RolePermissions-map aligneren + CrudOperation-TODO (create vs write) oplossen.
- Snapshot-test op exacte strings per rol.

## 3. OR: [Authz] /v1/me — juiste rol bij meerdere rollen
- `Roles.First()` vervangen door vaste prioriteit (centrale `RolePriority`).
- Displaynaam: `AlgemeenBeheerder` → `Algemeen Beheerder`.
- Tests voor rolvolgorde/permutaties.

## 4. OR: [Authz] canSelect op parameter-endpoints
- Nu enkel KeyType. Toevoegen op: capacitytypes, organisationclassificationtypes, labeltypes (bestaande filter omzetten), formalframeworktypes.
- KeyType = referentiepatroon (`CanSelect`/`UserPermitted`).
- Tests per endpoint.

## 5. OR: [Authz] canSelect in permissions-blokje
- Nu vlak veld; analyse wil `"permissions": { "canSelect": true }`.
- KeyType refactoren; nieuwe endpoints (story 4) meteen in dit formaat.
- Legacy `userPermitted` behouden. Swagger + contracttests.

## 6. OR: [Authz] Permissies afdwingen op parameters
- Geen van de 23 parameter-controllers heeft `RequiredPermissions` (attribuut doet niets zonder).
- Bescherming zit in 40 handlers met `RequiresOneOfRole(AlgemeenBeheerder, CjmBeheerder)`.
- Permissie toekennen (bv. `CanManageParameters`), `RequiredPermissions` op alle controllers, handlers naar permissie-evaluatie.
- PermissionMatrix-tests: 200 / 403 per type.

## 7. OR-3546 (bestaat): canViewParameters slim afleiden
- Afgeleid bij opbouw permissieset: `true` zodra minstens één `canView<Parameter>` aanwezig.
- Niet per rol hardcoderen; tests: 1 canView → true, 0 → afwezig.

---

**Volgorde**: 2 → 3 → 5 → 4 → 6 → 1 → 7
