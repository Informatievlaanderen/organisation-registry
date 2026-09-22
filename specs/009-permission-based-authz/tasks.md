# Tasks: Permission-Based Authorization

**Input**: Design documents from `/code/aiv/organisation-registry/specs/009-permission-based-authz/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/permission-check-api.md, quickstart.md

**Tests**: Included — feature spec explicitly requires unit + integration + policy tests (Constitution Principle V, plan §Testing).

**Organization**: Tasks grouped by user story. US1 = MVP.

---

## 🚢 SHIPPED STATUS (HEAD ac598dec3 — bijgewerkt na 12 commits sinds 17cd21335)

**Model C Reality**: Feature 009 shipped a first-class typed `IRestriction` layer (replacing the earlier `IUserRestrictionsProvider` design) with:
- `IRestrictionContext` marker + `IVlimpersManagedContext` capability
- `IRestriction` non-generic interface with `AllowListRestriction<T>`, `CompositeAndRestriction`, `RequireUnderVlimpersManagementRestriction`
- `KeyContext` record carrying org Vlimpers-management status + keytype ids
- `PermissionEntry` record with optional `IRestriction` storage
- `PermissionSet.IsSatisfiedFor(permission, context)` evaluation engine

**Shipped scope (update)**: Keys MVP + brede US2/US3-uitrol. Sinds 17cd21335 leverden 12 commits (35c9f4313…ac598dec3): permission checks op vrijwel alle command controllers (fd22418a9, caf9aecfd, fdefe4ac1), `RequiredPermissions` op het authorize-attribuut met obsolete role-pad (fd22418a9), policy-based handler authorization voor Building/Capacity/Contact/Function/Label/Location/Relation/Regulation/ClassificationType (f15fecbed), Body-policies (fdefe4ac1), Kbo- en VlimpersManagement-policies (cfe994ca9), scope-based permissions incl. `RequiresPermissionPolicy` (fe876c8c9), en read/detail-permissions (`OrganisationPermissions`, `BodyPermissions`, `ResourceEditPermissions`) op lijst- en detailendpoints (e87179270, 13dbbe2f8, ac598dec3). Uitgebreide PermissionMatrix-integratietests per permissie (b4411a30d, 35c9f4313, a32cb1079).

**Nog open**: restanten controller sweep (OrganisationKboController, ImportOrganisationsController, IsInRole-checks), 8 role-based policies (AdminOnly, EditDelegation, Import, RequiresRoles, Vlimpers, VlimpersOnly, BeheerderFor…×2), WellknownUser-dispatch (T035), docs (T038/T039), en de gaps t.o.v. het analysedocument "Rollen Wegwijs" (T045–T052).

---

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no cross-task dependencies)
- **[Story]**: US1 / US2 / US3, or blank for setup/foundational/polish
- Paths are absolute.

---

## Phase 1: Setup

**Purpose**: Prepare authorization-specific folders and shared test scaffolding.

- [x] T001 Create `src/OrganisationRegistry.Infrastructure/Authorization/` sub-folder layout (verify existing; no new project). Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/`.
- [x] T002 [P] Create `test/OrganisationRegistry.UnitTests/Authorization/` folder and add xUnit test-class stubs `RolePermissionMapTests.cs`, `ScopePermissionMapTests.cs`, `PermissionSetTests.cs`, `UserRestrictionsProviderTests.cs`. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/`.
- [x] T003 [P] Create `test/OrganisationRegistry.Api.IntegrationTests/Security/` folder with placeholder classes `EditApiPermissionTranslationTests.cs`, `TokenExchangePermissionTranslationTests.cs`, `ClientCredentialsScopePermissionTests.cs`. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/Security/`.

---

## Phase 2: Foundational (BLOCKS all user stories)

**Purpose**: Introduce the permission model primitives without wiring them anywhere yet. After this phase compiles and unit-tests pass, US1/US2/US3 can proceed in parallel.

- [x] T004 [P] Create `Permission` enum with all identifiers in PascalCase (`CanEditChildren, CanEditVlimpers, CanEditDelegations, CanAddLocations, CanAddContacts, CanAddBodies, CanEditBodies, CanRegisterBodies, CanManageKeys, CanManageLabels, CanManageCapacities, CanManageFormalFrameworks, CanManageOrganisationClassifications, CanManageRegulations, CanImport, CanRunScheduledJobs, CanReadOrafin, CanReadInfoEndpoints, CanReadConfiguration, CanEditOrganisationLabels, CanReadEvents, CanViewProjections`). Note: there is no `CanEditAll` admin-bypass permission — `AlgemeenBeheerder` carries every permission granularly via `RolePermissionMap`. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/Permission.cs`.
- [x] T005 [P] Create immutable `PermissionSet` value object (backed by `ImmutableHashSet<PermissionEntry>`) with `Empty`, `Union`, `Contains`, `IsSatisfiedFor(permission, context)`. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/PermissionSet.cs`.
- [x] T006 [P] **SUPERSEDED** — Originally designed as `IUserRestrictionsProvider` (JIT SQL fetch model). Shipped reality: restrictions became a first-class typed layer with `IRestrictionContext`, `IRestriction`, `AllowListRestriction<TContext>`, `CompositeAndRestriction`, `RequireUnderVlimpersManagementRestriction`, `KeyContext`, `PermissionEntry` carrying optional `IRestriction`, and `PermissionSet.IsSatisfiedFor(permission, context)` evaluation. No JIT SQL fetch per resource type; restrictions live in `PermissionEntry` and are evaluated within the permission check itself. See T006a–T006g below for shipped sub-tasks (all marked `[x]`).
  - [x] T006a [P] Create `IRestrictionContext` marker interface and `IVlimpersManagedContext` capability extension. Path: `src/OrganisationRegistry.Infrastructure/Authorization/Restrictions/IRestrictionContext.cs`.
  - [x] T006b [P] Create `IRestriction` non-generic interface with `IsOkWith(IRestrictionContext)`. Path: `src/OrganisationRegistry.Infrastructure/Authorization/Restrictions/IRestriction.cs`.
  - [x] T006c [P] Create `AllowListRestriction<TContext>` generic for id-whitelist restrictions. Path: `src/OrganisationRegistry.Infrastructure/Authorization/Restrictions/AllowListRestriction.cs`.
  - [x] T006d [P] Create `CompositeAndRestriction` for AND-composition of multiple restrictions. Path: `src/OrganisationRegistry.Infrastructure/Authorization/Restrictions/CompositeAndRestriction.cs`.
  - [x] T006e [P] Create `RequireUnderVlimpersManagementRestriction` singleton with `IMemoryCaches` for org-scope check. Path: `src/OrganisationRegistry.Infrastructure/Authorization/Restrictions/RequireUnderVlimpersManagementRestriction.cs`.
  - [x] T006f [P] Create `KeyContext` record with `IsUnderVlimpersManagement` and `KeyTypeIds` properties. Path: `src/OrganisationRegistry.Infrastructure/Authorization/Restrictions/KeyContext.cs`.
  - [x] T006g [P] Create `KeyRestrictions.VlimpersManaged(keyTypeIds)` factory method and `KeyRestrictions.AllowList(keyTypeIds)` helper. Path: `src/OrganisationRegistry.Infrastructure/Authorization/Restrictions/KeyRestrictions.cs`.
- [x] T007 Create `RolePermissionMap` static class exposing `For(IEnumerable<Role>, ILogger?)` and `For(IEnumerable<Role>, config, logger)` overloads. Encode the full mapping for `AlgemeenBeheerder, VlimpersBeheerder, DecentraalBeheerder, RegelgevingBeheerder, OrgaanBeheerder, Developer, CjmBeheerder, Orafin, AutomatedTask` per data-model.md. Fail-closed + Serilog error (throttled once/role/process) for unknown roles. `AlgemeenBeheerder` and `Developer` get the full permission set granularly (no `CanEditAll` bypass). `AutomatedTask` maps to `CanRunScheduledJobs` (event-sourcing immutability; scheduled-job migration deferred to T036). `VlimpersBeheerder` granted restricted `CanManageKeys` via `KeyRestrictions.VlimpersManaged(config.Authorization.KeyIdsAllowedForVlimpers)` (config-aware overload). Two overloads: base returns static map; config-aware adds restricted grants per role. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/RolePermissionMap.cs`.
- [x] T008 Create `ScopePermissionMap` static class exposing `For(IEnumerable<string>) : PermissionSet` keyed by exact scope strings from `AcmIdmConstants.Scopes` (`dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_orafinbeheerder`, `dv_organisatieregister_info`, `dv_organisatieregister_testclient`). `Info` → `CanReadInfoEndpoints` only. Fail-closed + Serilog error (throttled once/scope/process). Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/ScopePermissionMap.cs`.
- [x] T009 Modify `User` to carry `PermissionSet Permissions` (constructor arg) plus `HasPermission(Permission)`, `HasAnyPermission(params Permission[])`, and `IsSatisfiedFor(Permission, IRestrictionContext)`. Keep existing `Roles` property on the object surface for edge translation; internal domain code reads only `Permissions`. No `[Obsolete]` marker needed — `Roles` serves a purpose in edge layer (JWT emission, role-based test logic). Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/User.cs`.
- [x] T010 [P] Unit tests: `PermissionSetTests` (union, contains, IsSatisfiedFor with contexts, empty). Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/PermissionSetTests.cs`.
- [x] T011 [P] Unit tests: `RolePermissionMapTests` — one theory row per role verifying exact PermissionSet including unrestricted + restricted grants (e.g. VlimpersBeheerder CanManageKeys is restricted) + fail-closed on unknown. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/RolePermissionMapTests.cs`.
- [x] T012 [P] Unit tests: `ScopePermissionMapTests` — one theory row per scope + fail-closed + `Info`-scope isolation. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/ScopePermissionMapTests.cs`.
- [ ] T012a [P] Unit tests: `AllowListRestrictionTests`, `CompositeAndRestrictionTests`, `RequireUnderVlimpersManagementRestrictionTests`, `KeyRestrictionsTests` — verify restriction evaluation against `KeyContext` with in-scope and out-of-scope keytype ids and Vlimpers-management flags. Paths: `test/OrganisationRegistry.UnitTests/Authorization/Restrictions/`. **CORRECTIE (verificatie ac598dec3)**: eerste drie testklassen bestaan (+ `FormalFrameworkRestrictionTests`, `UserContextRestrictionTests`), maar `KeyRestrictionsTests.cs` ontbreekt — nog toe te voegen.
- [ ] T012b [P] Unit tests: `PermissionEntryTests` — restricted vs unrestricted entries, implicit conversion from bare `Permission`. Path: `test/OrganisationRegistry.UnitTests/Authorization/PermissionEntryTests.cs`. **CORRECTIE**: bestand bestaat niet; `PermissionEntry`-gedrag wordt deels indirect gedekt in `PermissionSetTests.cs`, maar dedicated tests ontbreken.
- [ ] T012c [P] Unit tests: `PermissionSetRestrictionTests` — `IsSatisfiedFor(permission, context)` with mixed restricted/unrestricted grants, absorbing logic (unrestricted grant absorbs restricted), fail-closed on missing permission or context mismatch. Path: `test/OrganisationRegistry.UnitTests/Authorization/PermissionSetRestrictionTests.cs`. **CORRECTIE**: bestand bestaat niet; `IsSatisfiedFor`-scenario's zitten gedeeltelijk in `PermissionSetTests.cs` — hernoem of vervolledig.

**Checkpoint**: `dotnet build` green; foundational unit tests green. US1/US2/US3 unblocked.

---

## Phase 3: User Story 1 — Rollen én scopes vertalen naar permissies aan de systeemrand (P1) 🎯 MVP

**Goal**: All three entry points (edit-api, token-exchange, bearer/CC scope) produce an `IUser` whose `Permissions` is built via `RolePermissionMap ∪ ScopePermissionMap`. Internally no role or scope-string is read anymore.

**Shipped scope**: ONLY Keys resource type shipped end-to-end (MVP). All other resource types (Labels, Capacities, FormalFrameworks, etc.) remain DEFERRED to future PRs following the same restriction pattern as Keys.

**Independent Test**: For each role and each CC scope, authenticate via the matching entry point and assert the resulting PermissionSet equals the documented mapping (via integration test harness). For Keys: verify KeyPolicy evaluation with `KeyContext` passes expected identities and rejects out-of-scope.

### Tests for US1

- [x] T013 [P] [US1] Integration test `EditApiPermissionTranslationTests` — baseline regression (option B): interactive user JWT via `ApiFixture.HttpClient` → `GET /v1/security` → 200 + `AlgemeenBeheerder` role. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/Security/EditApiPermissionTranslationTests.cs`.
- [x] T014 [P] [US1] Integration test `TokenExchangePermissionTranslationTests` — baseline regression (option B): Keycloak CC token for CJM/Orafin via token-exchange helper → `GET /v1/security` → 200 + expected role. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/Security/TokenExchangePermissionTranslationTests.cs`.
- [x] T015 [P] [US1] Integration test `ClientCredentialsScopePermissionTests` — baseline regression (option B): direct bearer for Test/CJM/Orafin CC clients → `GET /v1/security` → 200 + expected role. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/Security/ClientCredentialsScopePermissionTests.cs`.
- [x] T015a [US1] Integration test `KeyPolicyEndpointTests` — verify KeyPolicy enforcement at `OrganisationKeyController` and `KeyTypeController`. Test AlgemeenBeheerder access to all keytypes + VlimpersBeheerder restricted to Vlimpers-managed orgs + allowed keytype ids + out-of-scope denial. **NOTE (verificatie)**: `KeyPolicyEndpointTests.cs` bestaat niet meer; vervangen door `PermissionMatrix/Keys/Given_Roles_With_CanManageKeys.cs` + `Given_Roles_Without_CanManageKeys.cs` (commits 35c9f4313, a32cb1079 — oude `OrganisationKeysPermissionTests.cs` verwijderd). Dekking equivalent; taak blijft afgevinkt.

### Implementation for US1

- [x] T016 [US1] **NO-OP** — `User` ctor (T009) already derives `Permissions` from `roles` via `RolePermissionMap.For`. `RoleMapping.Map` still needed as edge-mapping `Role↔string` for JWT emission. Kept as-is.
- [x] T017 [US1] Added `ClaimsExtension.ToPermissionSet(this ClaimsPrincipal, ILogger? = null)` — reads `ClaimTypes.Role` + raw `AcmIdmConstants.Claims.Role` (strips `RolePrefix`), filters via `RoleMapping.Exists`, maps via `RoleMapping.Map` → `RolePermissionMap.For`; reads scopes from `AcmIdmConstants.Claims.Scope` split on `' '` → `ScopePermissionMap.For`; unions.
- [x] T018 [US1] **NO-OP** — `OrganisationRegistryTokenBuilder.ParseRoles` (line 171) emits `ClaimTypes.Role` via `RoleMapping.Map`. Downstream `ToPermissionSet` reads these claims at consume-time. No source change; keeps JWT payload minimal, avoids double-computation.
- [x] T019 [US1] **NO-OP** — `TokenExchangeClaimsTransformation.AddRoleClaim` (line 91) emits `ClaimTypes.Role` via `RoleMapping.Map`. Same rationale as T018.
- [x] T020 [US1] **NO-OP** — same rationale as T018/T019. `WellknownUsers.TestClient/Cjm/Orafin` already produce correct `Permissions` via `User` ctor → `RolePermissionMap.For(roles)`. Direct scope→PermissionSet cutover would break existing `IsInAnyOf(Role.*)` sites (e.g. `SecurityService.CanUseKeyType`) before US2/US3 migrate them. Final `WellknownUser`-based scope dispatch deletion deferred to T035 (US3), which the task text already anticipates. Info-scope dispatch also deferred to US3 alongside T035.
- [x] T021 [US1] Verified. `TokenExchangeConfiguration.RequiredScopes` is declared but **never consumed** anywhere in the codebase (orphan from feature 008); not this feature's concern. `TokenExchangeClaimsTransformation` has no scope handling — path relies on introspection + role-claim emission (T019 confirmed `AddRoleClaim` at line 91 still emits `ClaimTypes.Role`). Downstream `ToPermissionSet` (T017) consumes those claims correctly. Entry point #2 semantics preserved. No source change.
- [x] T022 [P] [US1] Unit test class `PermissionMapThrottleTests` — assert (a) unknown role/scope yields empty `PermissionSet` (fail-closed), (b) 100× same unknown value logs exactly 1 Serilog `Error` event, (c) two distinct unknown values log 2 events (per-key isolation). Use `Serilog.Sinks.TestCorrelator` or in-memory `List<LogEvent>` sink. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/PermissionMapThrottleTests.cs`.
- [x] T022a [P] [US1] Unit test class `KeyPolicyTests` — verify KeyPolicy evaluation against `KeyContext` with in-scope + out-of-scope keytype ids, Vlimpers-management gates, user with unrestricted `CanManageKeys` (e.g. AlgemeenBeheerder), user with restricted grant (VlimpersBeheerder), and user without permission (denial). Path (gecorrigeerd): `test/OrganisationRegistry.UnitTests/SecurityPolicy/KeyPolicyTests.cs`.

**Checkpoint**: US1 integration tests green. PermissionSet visible at all three entry points. MVP demoable: mapping table can be inspected by hitting a diagnostic endpoint or by test harness. Keys MVP end-to-end working.

---

## Phase 3.5: Keys Resource Type (Shipped MVP, Model C)

**Summary**: Keys shipped with full end-to-end restriction model. Following tasks are marked `[x]` to reflect shipped state.

- [x] T022b [US1] Create `PermissionEntry` record: `(Permission, IRestriction?)` backing `PermissionSet`. Structural equality for deduplication. Implicit conversion from bare `Permission` to unrestricted entry. Path: `src/OrganisationRegistry.Infrastructure/Authorization/PermissionEntry.cs`.
- [x] T022c [US1] Create `PermissionExtensions.RestrictedTo(Permission, IRestriction)` fluent helper for call sites. Path: `src/OrganisationRegistry.Infrastructure/Authorization/PermissionExtensions.cs`.
- [x] T022d [US1] Refactor `KeyPolicy` to use `IUser.IsSatisfiedFor(Permission, KeyContext)` evaluation. Policy constructor takes `isUnderVlimpersManagement` bool and `keyTypeIds` → builds `KeyContext` → evaluates permission + restriction in one pass. Path: `src/OrganisationRegistry/Handling/Authorization/KeyPolicy.cs`.
- [x] T022e [US1] Wire `AddOrganisationKeyCommandHandler` via `.WithKeyPolicy(command)` → pass `envelope.User` and `organisation.State.UnderVlimpersManagement + keyTypeId` to policy. Verify `KeyPolicy.Check(user).IsSuccessful`. Path: `src/OrganisationRegistry/Organisation/Keys/AddOrganisationKeyCommandHandler.cs`.
- [x] T022f [US1] Wire `UpdateOrganisationKeyCommandHandler` with same `.WithKeyPolicy(command)` pattern. Path: `src/OrganisationRegistry/Organisation/Keys/UpdateOrganisationKeyCommandHandler.cs`.
- [x] T022g [US1] Add `[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.CanManageKeys })]` to `OrganisationKeyController` (list, get, post, put, delete actions). **NOTE (verificatie)**: muterende acties zitten in `OrganisationKeyCommandController.cs` en dragen daar `RequiredPermissions = [Permission.CanManageKeys]`; read-acties op `OrganisationKeyController` blijven bewust `[OrganisationRegistryAuthorize]` (reads publiek/rolvrij, cf. e87179270 read-permissions model). Path: `src/OrganisationRegistry.Api/Backoffice/Organisation/Key/OrganisationKeyCommandController.cs`.
- [ ] T022h [US1] Add `[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.CanManageKeys })]` to `KeyTypeController`. **CORRECTIE (verificatie ac598dec3)**: `KeyTypeCommandController.cs` draagt enkel kaal `[OrganisationRegistryAuthorize]` — geen `RequiredPermissions`. Nog toe te voegen (of expliciet documenteren waarom keytype-parameterbeheer geen permissiecheck krijgt). Path: `src/OrganisationRegistry.Api/Backoffice/Parameters/KeyType/KeyTypeCommandController.cs`.

---

## Phase 4: User Story 2 — Controllers checken enkel algemene permissies (P2)

**Goal**: Every controller action uses `[OrganisationRegistryAuthorize(RequiredPermissions = new[] { … })]`; no rolname / scope-string references remain in controller layer. Elke rol (incl. `AlgemeenBeheerder`) krijgt granulair alle benodigde permissions via `RolePermissionMap` — geen admin-bypass in de attribute.

**Independent Test**: Static scan of controllers shows zero references to `Role.*` or `AcmIdmConstants.Scopes.*`; every authorized action carries `RequiredPermissions`; identities with matching permission pass, others get 403.

### Tests for US2

- [x] T023 [P] [US2] Integration test class `ControllerPermissionEnforcementTests` — parametrized over a representative sample of endpoints (one per permission), asserting 200 vs 403 based on identity's PermissionSet. **Gedekt door**: b4411a30d + caf9aecfd; aangevuld met `PermissionMatrix/*` mappen (Given_Roles_With/Without_CanManageX per resource: Bodies, Buildings, Capacities, Classifications, Contacts, FormalFrameworks, Functions, Kbo, Keys, Labels, Locations, Regulations, Relations, Vlimpers). Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/Security/ControllerPermissionEnforcementTests.cs` + `Security/PermissionMatrix/`.
- [x] T024 [P] [US2] Unit test class `OrganisationRegistryAuthorizeAttributeTests` — attribute admits identity when PermissionSet contains any of `RequiredPermissions`; identity without required permission gets 403. Geen admin-short-circuit. **Gedekt door**: fd22418a9 (+ caf9aecfd). Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/OrganisationRegistryAuthorizeAttributeTests.cs`.

### Implementation for US2

- [x] T025 [US2] Extend `OrganisationRegistryAuthorizeAttribute` with `Permission[] RequiredPermissions { get; set; }` + fallback op policy-checks bij parameterloos gebruik; legacy role-based ctor gemarkeerd `[Obsolete]`. **Gedekt door**: fd22418a9. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/Security/OrganisationRegistryAuthorizeAttribute.cs`.
- [x] T026 [US2] **Controller sweep** — grotendeels uitgevoerd: organisatie-commandcontrollers (fd22418a9), body/delegation-controllers (caf9aecfd, fdefe4ac1), Kbo/Vlimpers (cfe994ca9), contacts (a32cb1079). **Restanten** (nog role-based, zie T043): `OrganisationKboController` (2× `Role.*`-attribuut), `ImportOrganisationsController` (1×).
- [ ] T027 [US2] Remove all direct `IUser.Roles.Contains(...)` / role-string comparisons / `AcmIdmConstants.Scopes.*` comparisons from controller code. **Status**: nog ~18 `IsInAnyOf`/`IsInRole`-sites, o.a. `OrganisationDetailCommandController`, `BodyDetailCommandController`, `PersonDetailController` (`IsInRole(RoleMapping.Map(...))`). Zie ook T043.
- [ ] T028 [US2] Modify `PolicyNames.cs` (if it enumerates authorization policies) to align with permission ids; delete obsolete role-based policy names. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/PolicyNames.cs`.

**Checkpoint**: `grep -R "Role\." src/OrganisationRegistry.Api/` returns only edge-translation files (RoleMapping, ClaimsExtension, TokenBuilder, TokenExchange). Controller sweep clean.

---

## Phase 5: User Story 3 — Policies checken enkel restricties/scope (P3)

**Goal**: All resource-type policies remove role/scope-string checks; they gate on resource-level scope only via `IRestrictionContext` evaluation. **Status update (ac598dec3)**: 15 policies gemigreerd naar `IsSatisfiedFor` (Body, Building, Capacity, Contact, FormalFramework, Function, Kbo, Key, Label, Location, OrganisationClassificationType, RegisterBody, Regulation, Relation, VlimpersManagement — f15fecbed, fdefe4ac1, cfe994ca9). Nog role-based: AdminOnly, EditDelegation, Import, RequiresRoles, Vlimpers, VlimpersOnly, BeheerderForOrganisationButNotUnderVlimpersManagement, BeheerderForOrganisationRegardlessOfVlimpers (zie T044).

**Independent Test**: Policy-unittests per resource in `test/OrganisationRegistry.UnitTests/SecurityPolicy/`.

### Tests for US3

- [x] T029 [P] [US3] Unit test class `LabelPolicyRestrictionTests` — restriction context for label operations. **Gedekt door**: f15fecbed/fe876c8c9 via bestaande `SecurityPolicy/LabelPolicyTests.cs` (LabelPolicy nu op `IsSatisfiedFor` + `LabelContext`). Path (gecorrigeerd): `test/OrganisationRegistry.UnitTests/SecurityPolicy/LabelPolicyTests.cs`.
- [x] T030 [P] [US3] Unit test class `VlimpersPolicyRestrictionTests` — restriction context for Vlimpers operations. **Gedeeltelijk gedekt door**: cfe994ca9 (`VlimpersManagementPolicyTests.cs` voor `CanManageVlimpers`). Resterende Vlimpers-policies (VlimpersPolicy, VlimpersOnlyPolicy, BeheerderFor…) → T044. Path (gecorrigeerd): `test/OrganisationRegistry.UnitTests/SecurityPolicy/VlimpersManagementPolicyTests.cs`.

### Implementation for US3

- [x] T031 [US3] Create policy contexts for non-key resource types. **Gedekt door**: f15fecbed (`LabelContext`, `CapacityContext`, `ClassificationTypeContext`), fdefe4ac1 (`BodyContext`, `DecentraalBodyRestriction`), e87179270 (`FormalFrameworkContext`), plus `OrganisationContext`, `UserContext`, `DecentraalOrganisationRestriction`, `NotAllowListRestriction`. Path: `src/OrganisationRegistry.Infrastructure/Authorization/Restrictions/`.
- [x] T032 [US3] Refactor label policies (`LabelPolicy`, etc.) to use `IRestrictionContext` evaluation. **Gedekt door**: f15fecbed. Path: `src/OrganisationRegistry/Handling/Authorization/LabelPolicy.cs`.
- [ ] T033 [US3] Refactor Vlimpers policies (`VlimpersPolicy`, `VlimpersOnlyPolicy`, `BeheerderForOrganisationRegardlessOfVlimpersPolicy`) to use restriction evaluation. **Status**: `VlimpersManagementPolicy` (nieuw, cfe994ca9) gebruikt `IsSatisfiedFor`; de drie genoemde policies zijn nog role-based.
- [x] T034 [US3] Refactor remaining policies. **Gedekt door**: f15fecbed (Building, Capacity, Contact, Function, Location, Relation, Regulation, ClassificationType), fdefe4ac1 (BodyPolicy vervangt AddBodyPolicy/EditBodyPolicy; RegisterBodyPolicy), cfe994ca9 (KboPolicy), fe876c8c9 (`RequiresPermissionPolicy` + handler-wiring). **Restanten** → T044 (AdminOnly, EditDelegation, Import, RequiresRoles).
- [ ] T035 [US3] Delete `WellknownUser`-based scope→user dispatch entirely from `SecurityService` (already touched in T020); confirm no residual references. Nog open (`WellknownUsers.cs` bestaat nog; `MeController` vergelijkt met `WellknownUsers.Nobody`).

**Checkpoint**: 15/23 policies op permissie/restrictie-model; restanten in T044.

---

## Phase 5.5: Policies Refactor Status (Updated for Shipped MVP)

- [ ] T036 [P] **WON'T DO (event-sourcing immutability)** — `AutomatedTask` role remains in the `Role` enum and maps to `CanRunScheduledJobs` in `RolePermissionMap`. Rationale: event-sourcing means historical events are immutable; if `Role.AutomatedTask` appears in any persisted event payload (e.g. `RoleAssigned`), the enum value MUST stay for deserialization. With zero historical usage verified and a clear migration path to Client Credentials (WellknownUsers to deprecate), the enum stays. Deletion deferred to a future release after scheduled-job and KBO-sync services migrate away. Do NOT delete; mark notes in code. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/Role.cs`.
- [ ] T037 [P] **NO-OP** — `Roles` property remains on `User` class surface for edge layer (JWT emission, role-based test logic, `IsInAnyOf` calls). No `[Obsolete]` marker needed; marked as intentional in code comments. Internal domain code reads only `Permissions`. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/User.cs`.
- [ ] T038 [P] Update `AGENTS.md` recent-changes section with permission-model summary: "009-permission-based-authz: Shipped Model C — first-class typed `IRestriction` layer (AllowListRestriction, CompositeAndRestriction, RequireUnderVlimpersManagementRestriction) with `KeyContext`, PermissionEntry-based storage, and `PermissionSet.IsSatisfiedFor(permission, context)` evaluation. KeyPolicy + Keys MVP end-to-end. Other policies deferred to follow the same pattern." Path: `/code/aiv/organisation-registry/AGENTS.md`.
- [ ] T039 [P] Add centralized permission catalog doc referencing spec + data-model. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/README.md`.
- [ ] T040 **PARTIAL** — Run quickstart.md validation: authenticate via all three entry points against a local dev instance; confirm PermissionSet + KeyContext restriction behavior (MVP Keys). Other resource types TBD in future PRs. Path: `/code/aiv/organisation-registry/specs/009-permission-based-authz/quickstart.md`.
- [ ] T041 **PARTIAL** — Static analysis pass (MVP scope): assert `grep -R "AcmIdmConstants.Scopes" src/` shows only `ScopePermissionMap.cs`; `grep -R "Role\." src/OrganisationRegistry.Api/` shows only edge files except for `IsInAnyOf(Role.*)` calls in non-controller domain/policy code (still used, planned for US3 sweep). Success criterion SC-006 partial (controller layer clean, policies deferred).
- [ ] T042 **PARTIAL** — Run full test suite (`dotnet test`) + verify no regression in existing authorization tests. Keys tests pass; other policy tests pending US3 refactor.

---

## Phase 6: Follow-up — Restanten na US2/US3-uitrol (nieuw, ac598dec3)

**Purpose**: Afronden van de sweep die de 12 commits sinds 17cd21335 grotendeels leverden.

- [ ] T043 [US2] Vervang de laatste role-based controller-checks door permissies: `[OrganisationRegistryAuthorize(Role.*)]` op `src/OrganisationRegistry.Api/Backoffice/Organisation/Kbo/OrganisationKboController.cs` (2×) en `src/OrganisationRegistry.Api/Import/Organisations/ImportOrganisationsController.cs` (1×); `IsInRole(RoleMapping.Map(...))`-checks in `Backoffice/Organisation/Detail/OrganisationDetailCommandController.cs`, `Backoffice/Body/Detail/BodyDetailCommandController.cs`, `Backoffice/Person/Detail/PersonDetailController.cs`.
- [ ] T044 [US3] Migreer de resterende 8 role-based policies naar permissie/restrictie-evaluatie: `AdminOnlyPolicy`, `EditDelegationPolicy`, `ImportPolicy`, `RequiresRolesPolicy` (uitfaseren), `VlimpersPolicy`, `VlimpersOnlyPolicy`, `BeheerderForOrganisationButNotUnderVlimpersManagementPolicy`, `BeheerderForOrganisationRegardlessOfVlimpersPolicy`. Path: `src/OrganisationRegistry/Handling/Authorization/`.
- [ ] T044a [P] Voeg ontbrekende `KeyRestrictionsTests.cs` toe in `test/OrganisationRegistry.UnitTests/Authorization/Restrictions/` (zie T012a-correctie).
- [ ] T044b [P] Voeg unit tests toe voor nieuwe restriction-contexten zonder dedicated tests: `BodyContext`/`DecentraalBodyRestriction`, `CapacityContext`, `ClassificationTypeContext`, `LabelContext`, `OrganisationContext`, `DecentraalOrganisationRestriction`, `NotAllowListRestriction`. Path: `test/OrganisationRegistry.UnitTests/Authorization/Restrictions/`.

---

## Phase 7: Gaps t.o.v. functionele analyse "Rollen Wegwijs" (nieuw)

**Purpose**: Aansluiting op het analysedocument (Rollen Wegwijs): /v1/me-contract, resource-rechten in responses, referentiedata-canSelect, nieuwe rollen en businessregels.

- [ ] T045 `/v1/me` endpoint afstemmen op analysecontract: response met `name`, `role` (eerste rol volgens vaste rolvolgorde uit de analyse) en `permissions` als `<resource>:<action>`-strings (bv. `organisations:create`, `body.info:create`, `parameters:read/write`, `imports:write`, `reports:read`); enkel globale rechten, geen contextuele afbakeningen; publieke leesrechten niet opnemen. Bestaande `MeController` (`src/OrganisationRegistry.Api/Auth/MeController.cs` + `RolePermissions.Resolve`) reviewen/aanpassen aan dit contract + integratietests.
- [ ] T046 [P] Resource-permissions (`permissions.can...`) op organisatieresponse vervolledigen conform analyse: `canEdit`, `canDelete`, `canManageChildren`, `canManageContacts`, `canManageLocations`, `canManageBuildings`, `canManageFunctions`, `canManageCapacities`, `canManageNames`, `canManageClassifications`, `canManageFormalFrameworks`, `canManageKeys`, `canManageRelations`. Basis bestaat (`OrganisationPermissions.cs`, ac598dec3/e87179270) — audit tegen de rechtenmatrix en vul gaten aan. Path: `src/OrganisationRegistry.Api/Backoffice/Organisation/OrganisationPermissions.cs` + `Detail/OrganisationResponse.cs`.
- [ ] T047 [P] Resource-permissions op orgaanresponse conform analyse: `canEdit`, `canDelete`, `canManageContacts`, `canManageSeats`, `canManageMandates`, `canManageLifecycle`, `canManageOrganisations`, `canManageFormalFrameworks`, `canManageMep`, `canManageClassifications`. Basis bestaat (`BodyPermissions.cs`, `BodyResponse.cs`, e87179270/ac598dec3) — audit en vervolledig. Path: `src/OrganisationRegistry.Api/Backoffice/Body/BodyPermissions.cs` + `Detail/BodyResponse.cs`.
- [ ] T048 [P] `permissions`-object op onderliggende resource-items in lijstresponses: hoedanigheden (`canEdit`,`canDelete`), classificaties (`canEdit`), benamingen/labels (`canEdit`), toepassingsgebieden (`canEdit`,`canDelete`), sleutels (`canEdit`, naast legacy `isEditable`). Basis: `ResourceEditPermissions.cs` + list queries (e87179270, 13dbbe2f8) — vervolledig `canDelete` waar de analyse dat vereist. Paths: `src/OrganisationRegistry.Api/Backoffice/Organisation/{Capacity,OrganisationClassification,Label,FormalFramework,Key}/…ListQuery.cs`.
- [ ] T049 Referentiedata-rechten `permissions.canSelect` per waarde met `?forOrganisationId=` op `/v1/keytypes`, `/v1/capacitytypes`, `/v1/classificationtypes`, `/v1/labeltypes`, `/v1/formalframeworktypes`; Vlimpers- en typed afbakeningen backend-side; bij opslaan hervalideert de backend het gekozen type. Paths: `src/OrganisationRegistry.Api/Backoffice/Parameters/{KeyType,CapacityType,OrganisationClassificationType,LabelType,FormalFrameworkType}/…Controller.cs`.
- [ ] T050 Nieuwe rollen uit analyse (beslissing 23 juli 2026): `VoMedewerker` (publieke rol + hoedanigheden) en `DecentraalBeheerderVo` (decentraal beheerder zonder contactgegevens-beheer van lokale besturen; exacte afbakening TBD) toevoegen aan `Role`-enum + `RoleMapping` + `RolePermissionMap` + rechtenmatrix-tests. Paths: `src/OrganisationRegistry.Infrastructure/Authorization/{Role.cs,RoleMapping.cs,RolePermissionMap.cs}`, `test/OrganisationRegistry.UnitTests/Authorization/RolePermissionMapTests.cs`.
- [ ] T051 Businessregels uit analyse afdwingen als restricties/policies + tests: (a) KBO-data read-only in Wegwijs (Rechtsvorm-classificatie, Maatschappelijke zetel-locatie, Formele naam-benaming, KBO-nummer); (b) OVO-afbakening DecentraalBeheerder (eigen OVO-code + dochters); (c) Vlimpers-afbakening (enkel organisaties met Vlimpers = true); (d) orgaan-afbakening (organen gelinkt aan eigen OVO-code + dochters); (e) delete = beëindigen geldigheidsperiode — echte verwijdering enkel voor AlgemeenBeheerder op: bankrekeningnummers, functies, hoedanigheden, toepassingsgebieden, delegaties, organisatielocatie. Paths: `src/OrganisationRegistry.Infrastructure/Authorization/Restrictions/` + `src/OrganisationRegistry/Handling/Authorization/`.
- [ ] T052 Consistentieregel "resource-rechten nooit ruimer dan bovenliggend recht": garandeer backend-side dat bv. `sleutel.permissions.canEdit` nooit `true` is wanneer `organisatie.permissions.canManageKeys` `false` is (idem voor alle can…-poorten); voeg unit-/integratietests toe die inconsistente responses aantonen als bug. Paths: `src/OrganisationRegistry.Api/Backoffice/Organisation/OrganisationPermissions.cs`, `ResourceEditPermissions.cs`, tests in `test/OrganisationRegistry.Api.IntegrationTests/Security/`.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: no deps.
- **Foundational (Phase 2)**: after Setup. Blocks US1/US2/US3.
- **US1 (Phase 3)**: after Foundational. MVP shipped complete; all tasks marked `[x]`.
- **US1 Keys MVP (Phase 3.5)**: after US1, contained within c3b0d4af5. All tasks marked `[x]`.
- **US2 (Phase 4)**: after US1 complete. Grotendeels geleverd (fd22418a9, caf9aecfd, fdefe4ac1, a32cb1079); restanten in T043.
- **US3 (Phase 5)**: after US2. Grotendeels geleverd (f15fecbed, fdefe4ac1, cfe994ca9, fe876c8c9); restanten in T044/T035.
- **Follow-up (Phase 6)** en **Analyse-gaps (Phase 7)**: na US2/US3-restanten; T045–T049 kunnen parallel.
- **Polish (Phase 5.5)**: docs (T038/T039) en validatie (T040–T042) nog open.

### Story-level Dependencies

- US2 (controllers) depends on US1 (PermissionSets exist to check). US2 blocked by MVP cutover decision.
- US3 (policies) depends on US1 (identities have PermissionSets) but is orthogonal to US2. Deferred.
- KeyPolicy (US1 MVP) complete; other policies follow the same `IRestrictionContext` pattern in future PRs.

### Parallel Opportunities (Historical; MVP complete)

- Foundational: T004, T005, T006a–g fully parallel.
- US1: T013, T014, T015, T015a parallel (different test files). T018, T019, T020 touch different entry-point files → parallel.
- US1 Keys MVP: T022b–h wired; KeyPolicy shipped.
- US2/US3: Deferred to future PRs; current codebase does not execute US2/US3 tasks in parallel.

---

## Implementation Strategy

### MVP (US1 + Keys — SHIPPED)

1. Phase 1 → Phase 2 → Phase 3 → Phase 3.5 complete.
2. Delivered: PermissionSets flowing at all three entry points; KeyPolicy integrated end-to-end; Keys controller guarded with `RequiredPermissions`.
3. Status: internal authorization refactoring shipped; business logic (add key, update key) protected by `KeyPolicy` with restriction evaluation.

### Incremental Delivery (PARTIAL COMPLETE)

1. ✅ Setup + Foundational → foundation ready.
2. ✅ US1 → PermissionSets flowing at edges.
3. ✅ US1 Keys MVP → KeyPolicy + Keys endpoints protected.
4. ✅ US2 → controllers grotendeels op `RequiredPermissions` (fd22418a9, caf9aecfd, fdefe4ac1; restanten T043).
5. ✅ US3 → 15/23 policies op permissie/restrictie-model (f15fecbed, fdefe4ac1, cfe994ca9; restanten T044, T035).
6. ⏸ Polish → cleanup + docs (partial; T036 WON'T DO, T037 NO-OP, T038–T042 open).
7. ⏳ Analyse-gaps (Phase 7, T045–T052) → /v1/me-contract, resource-permissions, canSelect, nieuwe rollen, businessregels.

### Future PR Strategy (US2/US3)

- After MVP cutover: sweep all controllers with `RequiredPermissions` attribute mapping (US2).
- Refactor non-key policies (Labels, Vlimpers, etc.) to follow the KeyPolicy + KeyContext pattern with new resource-specific contexts and restrictions (US3).
- Migration path: Each policy type (Label, Vlimpers, etc.) gets own `IRestrictionContext` subclass and restriction implementations, then wired via `IUser.IsSatisfiedFor`.

---

## Notes

- **MVP Release**: US1 + Keys MVP shipped c3b0d4af5. Partial authorization refactor — permission translation + Keys policy only.
- **Cutover to full enforcement** (future): US2+US3 must ship together to avoid a partial state where edges emit PermissionSets but non-key downstream still reads roles.
- No new NuGet packages required.
- No event-store changes.
- Domain naming Dutch preserved; permission ids PascalCase English.
- Fail-closed enforced at both maps; Serilog throttled to prevent log flooding.
- **SC-006 acceptance** (partial): `RolePermissionMap.cs` + `ScopePermissionMap.cs` remain as internal role/scope readers. Controllers do not reference roles directly (Keys done; others deferred). Non-key policies still reference roles (deferred to US3).
- **Restrictions layer** is the shipped alternative to the earlier `IUserRestrictionsProvider` design. No JIT SQL fetching; restrictions are immutable typed objects stored in `PermissionEntry` and evaluated against operation contexts.
