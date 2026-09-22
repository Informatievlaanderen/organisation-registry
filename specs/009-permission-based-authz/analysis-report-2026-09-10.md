# Analyse-rapport feature 009 — permission-based authorization

**Bron**: `/speckit.analyze` op branch `009-perm-based-2` (HEAD `ac598dec3`), 10 september 2026.
**Artefacten**: `specs/009-permission-based-authz/` — spec.md, plan.md, tasks.md + constitution.
**Verdict**: geen CRITICAL issues — implementatie mag doorgaan, maar **de spec loopt achter op wat er intussen gebouwd is**. Het meeste hieronder is documentatie-schuld, geen code-schuld.

---

## Leeswijzer: de belangrijkste task-ids

Voor wie tasks.md niet vanbuiten kent — de taken waar het rapport naar verwijst:

| Taak | Wat het is |
|---|---|
| **T006** | De oorspronkelijk geplande `IUserRestrictionsProvider`: restricties (welke org/orgaan-ids iemand mag bewerken) zouden *just-in-time* uit SQL-projecties gehaald worden op het moment dat een policy ze nodig heeft. **SUPERSEDED**: gebouwd is "Model C" — restricties zijn typed objecten (`AllowListRestriction`, `RequireUnderVlimpersManagementRestriction`, …) die al ín de `PermissionSet` van de user zitten en geëvalueerd worden via `IsSatisfiedFor(permission, context)`. Geen JIT SQL-fetch. |
| **T012a/b/c** | Unit tests voor de restriction-laag (`KeyRestrictionsTests`, `PermissionEntryTests`, `PermissionSetRestrictionTests`). Stonden afgevinkt, maar de testbestanden **bestaan niet** — teruggezet naar open. |
| **T022h** | `RequiredPermissions` toevoegen op `KeyTypeCommandController`. Die draagt nu enkel een kaal `[OrganisationRegistryAuthorize]` zonder permissiecheck. |
| **T027 / T043** | Restanten van de controller-sweep: nog ~18 `IsInAnyOf`/`IsInRole`-checks in controllercode, plus role-based attributen op `OrganisationKboController` en `ImportOrganisationsController`. |
| **T035** | `WellknownUsers`-mechanisme (scope → hardcoded user zoals Cjm/Orafin/TestClient) verwijderen uit `SecurityService`. Nog in gebruik, o.a. door `MeController`. |
| **T036** | Voornemen om de rol `AutomatedTask` uit het `Role`-enum te schrappen. **WON'T DO**: door event sourcing zijn historische events immutable — als `Role.AutomatedTask` in een gepersisteerd event-payload zit, moet de enum-waarde blijven bestaan voor deserialisatie. De rol blijft dus, gemapt op enkel `CanRunScheduledJobs`; echte verwijdering pas nadat scheduled jobs en KBO-sync naar Client Credentials gemigreerd zijn. |
| **T037** | Voornemen om `User.Roles` te verwijderen/`[Obsolete]` te maken. **NO-OP**: de property blijft bewust bestaan voor de *edge-laag* (JWT-emissie, token exchange, testlogica). Interne domeincode leest enkel nog `Permissions`. |
| **T040–T042** | Kwaliteitspoort: quickstart-validatie tegen lokale dev-instance, statische scan (geen `Role.*`/scope-strings buiten edge-files), volledige `dotnet test`-regressierun. Alle drie nog niet uitgevoerd. |
| **T044 (+a/b)** | De 8 resterende role-based policies migreren (`AdminOnly`, `EditDelegation`, `Import`, `RequiresRoles`, `Vlimpers`, `VlimpersOnly`, 2× `BeheerderFor…`) + ontbrekende tests voor de nieuwe restriction-contexten. |
| **T045–T052** | **Phase 7, nieuw**: het frontend-contract uit het analysedocument "Rollen Wegwijs" — `/v1/me` (name/role/permissions), `can…`-velden op organisatie/orgaan/onderliggende resources, `canSelect` op referentiedata, nieuwe rollen `VoMedewerker` + `DecentraalBeheerderVo`, businessregels (KBO read-only, OVO/Vlimpers/orgaan-afbakening, delete = beëindigen geldigheidsperiode), consistentieregel "resource-recht nooit ruimer dan bovenliggend recht". |

---

## Bevindingen

### HIGH — spec en realiteit spreken elkaar tegen

| ID | Waar | Probleem | Aanbeveling |
|---|---|---|---|
| **I1** | spec FR-017/FR-018, plan.md | Spec en plan beschrijven nog het **JIT-restrictiemodel** (T006: request-scoped memoisatie, fetch uit SQL-projecties). Gebouwd is Model C: typed `IRestriction`-objecten in de `PermissionSet` zelf. FR-017/FR-018 zijn de facto vervallen, maar dat staat nergens. | FR-017/FR-018 en plan.md (Summary, Structure, Performance Goals) amenderen naar Model C, of een expliciete supersede-notitie in spec.md. |
| **I2** | spec FR-016 ↔ T036 | FR-016 eist dat `AutomatedTask` **verdwijnt**; T036 besliste terecht WON'T DO (event-immutabiliteit, zie leeswijzer). Directe tegenspraak — en de T036-rationale wint, want die volgt Constitution-principe I (append-only events). | FR-016 herformuleren: "AutomatedTask verdwijnt uit actieve toekenning; het enum blijft voor event-deserialisatie; verwijdering na CC-migratie van scheduled jobs/KBO-sync." |
| **I3** | spec FR-015 ↔ T009/T037 | FR-015 eist dat na de vertaalstap **geen rollen meer** in het security-object zitten. In werkelijkheid behoudt `User` bewust zijn `Roles`-property voor de edge-laag (T037 NO-OP). Het nul-rollen-doel wordt niet gehaald en is ook niet gepland. | FR-015 versoepelen tot "interne domeincode leest uitsluitend `Permissions`; `Roles` blijft enkel als edge-laag-oppervlak" — of een taak toevoegen die Roles echt van het interne oppervlak haalt. |
| **I4** | spec Edge Cases ↔ tasks.md | Spec eist **één cutover-release** zonder overgangsperiode met dubbele checks. Realiteit: incrementele uitrol (Keys-MVP → brede sweep), het obsolete role-pad zit nog in het authorize-attribuut, en T043/T044 staan open. Er bestaat dus feitelijk een dual-model. | Edge-case bijwerken naar de incrementele strategie, met de expliciete eis dat T043 + T044 samen landen vóór release (staat al als noot in tasks.md: "US2+US3 must ship together"). |
| **G1** | tasks.md Phase 7 | **T045–T052 hebben geen enkele FR/SC in spec.md.** Ze komen uit het externe analysedocument "Rollen Wegwijs" (Confluence-export, niet in de repo). 8 taken zonder requirement-basis. | Ofwel spec.md verrijken met FR's + user stories voor het frontend-contract, ofwel **Phase 7 afsplitsen naar een eigen feature 010** (aanbevolen: het is een eigen werkstroom met eigen stories). |

### MEDIUM

| ID | Waar | Probleem | Aanbeveling |
|---|---|---|---|
| **I5** | tasks.md intern | Verouderde passages ("ONLY Keys shipped", "US2/US3 deferred") staan naast de bijgewerkte status (15/23 policies gemigreerd, brede controller-sweep). Twee statusbeelden in één file. | Verouderde passages in Phase 3-header, §Parallel Opportunities en §Future PR Strategy opruimen. |
| **I7** | T045 ↔ FR-011 | Het `/v1/me`-contract gebruikt permissies als `<resource>:<action>`-strings (`org.organisations:create`), terwijl FR-011 intern PascalCase vastlegt (`CanManageKeys`). De mapping tussen die twee naamruimten is **nergens gespecificeerd**. | Mapping intern ↔ extern documenteren in spec/contracts vóór T045 gebouwd wordt. |
| **G2** | FR-017/FR-018 | Gevolg van I1: geen enkele taak dekt deze requirements nog. | Zelfde remediëring als I1. |
| **G3** | T012a–c, T044a/b, T022h | Geleverde code (restriction-laag, `PermissionEntry`, `KeyTypeCommandController`) mist dedicated tests/attributen. Constitution-principe V (tests verplicht) is daar nog niet vervuld. | Testschuld prioriteren vóór verdere Phase 7-uitbouw. |
| **U1** | T050 | Nieuwe rol `DecentraalBeheerderVo`: afbakening is "TBD" in het analysedocument — onopgeloste placeholder in een taak die enum + mapping + tests moet opleveren. | Afbakening eerst beslissen (clarify met analist) vóór dit een story wordt. |
| **U2** | T045/T049/T051 | Taken verwijzen naar de rechtenmatrix en "vaste rolvolgorde uit de analyse" — een document dat **niet in de repo staat**. Onreproduceerbaar voor toekomstige uitvoerders. | Minimaal de beslistabellen (rechtenmatrix, rolvolgorde) opnemen in de spec-map. |
| **C1** | Constitution V ↔ T042 | 39 taken staan als geleverd gemarkeerd, maar de volledige `dotnet test`-regressierun (T042) is nog niet gedraaid. Geen MUST-schending, wel een open kwaliteitspoort. | T042 draaien en het resultaat vastleggen vóór merge richting release. |

### LOW

| ID | Probleem |
|---|---|
| **I6** | plan.md zegt "~18 permissies", T004 definieert er 22 (cosmetisch). |
| **U3** | T036/T037 staan als open `[ ]` terwijl ze WON'T DO/NO-OP zijn — vertekent de open-telling. Markeren als afgehandeld-met-label. |
| **A1** | spec-voorbeelden gebruiken camelCase (`canEditChildren`) waar de clarificatie PascalCase vastlegt. |
| **A2** | SC-005 ("nieuwe dev vindt binnen 15 min de juiste permissie") heeft geen gedefinieerde validatie-oefening. |

---

## Coverage

| Metric | Waarde |
|---|---|
| Requirements (FR + SC) | 24 (FR-013 out-of-scope) |
| Taken (incl. subtaken) | 71 |
| Coverage | 87,5% — FR-016/017/018 zonder geldige dekking (zie I1/I2) |
| Duplicaten | 0 |
| **Critical** | **0** |

Requirements zonder (geldige) taak: **FR-016** (T036 = WON'T DO, tegengestelde richting), **FR-017/FR-018** (T006 SUPERSEDED). Taken zonder requirement: **T045–T052** (Phase 7).

---

## Aanbevolen volgorde

1. **Spec-amendement** — FR-015/016/017/018 + cutover-edge-case bijwerken naar de gebouwde realiteit (I1–I4, G2). Via `/speckit.specify` refinement.
2. **Phase 7 legitimeren** (G1) — FR's toevoegen of afsplitsen naar **feature 010**; rechtenmatrix + rolvolgorde in de repo vastleggen (U2); `DecentraalBeheerderVo`-afbakening klaren (U1); naamruimte-mapping intern ↔ extern specificeren (I7).
3. **plan.md bijwerken** — JIT-model → Model C, permissie-aantal actualiseren (I6).
4. **tasks.md opschonen** — verouderde deferred-passages (I5), T036/T037-notatie (U3).
5. **Kwaliteitspoort sluiten** — T012a–c, T044a/b, T022h + volledige `dotnet test` (C1, G3) vóór release.
