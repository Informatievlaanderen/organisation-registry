# Tasks: Permission-Based Authorization

**Input**: Design documents from `/code/aiv/organisation-registry/specs/009-permission-based-authz/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/permission-check-api.md, quickstart.md

**Tests**: Included — feature spec explicitly requires unit + integration + policy tests (Constitution Principle V, plan §Testing).

**Organization**: Tasks grouped by user story. US1 = MVP.

---

## 🚢 SHIPPED STATUS (HEAD c3b0d4af5)

**Model C Reality**: Feature 009 shipped a first-class typed `IRestriction` layer (replacing the earlier `IUserRestrictionsProvider` design) with:
- `IRestrictionContext` marker + `IVlimpersManagedContext` capability
- `IRestriction` non-generic interface with `AllowListRestriction<T>`, `CompositeAndRestriction`, `RequireUnderVlimpersManagementRestriction`
- `KeyContext` record carrying org Vlimpers-management status + keytype ids
- `PermissionEntry` record with optional `IRestriction` storage
- `PermissionSet.IsSatisfiedFor(permission, context)` evaluation engine

**Shipped scope**: ONLY Keys resource type end-to-end (MVP). Phase 1 (Setup) + Phase 2 (Foundational) + Phase 3 (US1) + **Phase 3.5 (Keys MVP)** complete. All tasks marked `[x]`.

**Deferred**: US2 (full controller sweep), US3 (other 16 policies). Future PRs will apply the same restriction pattern to Labels, Vlimpers, Capacities, etc.

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
- [x] T012a [P] Unit tests: `AllowListRestrictionTests`, `CompositeAndRestrictionTests`, `RequireUnderVlimpersManagementRestrictionTests`, `KeyRestrictionsTests` — verify restriction evaluation against `KeyContext` with in-scope and out-of-scope keytype ids and Vlimpers-management flags. Paths: `test/OrganisationRegistry.UnitTests/Authorization/Restrictions/`.
- [x] T012b [P] Unit tests: `PermissionEntryTests` — restricted vs unrestricted entries, implicit conversion from bare `Permission`. Path: `test/OrganisationRegistry.UnitTests/Authorization/PermissionEntryTests.cs`.
- [x] T012c [P] Unit tests: `PermissionSetRestrictionTests` — `IsSatisfiedFor(permission, context)` with mixed restricted/unrestricted grants, absorbing logic (unrestricted grant absorbs restricted), fail-closed on missing permission or context mismatch. Path: `test/OrganisationRegistry.UnitTests/Authorization/PermissionSetRestrictionTests.cs`.

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
- [x] T015a [US1] Integration test `KeyPolicyEndpointTests` — verify KeyPolicy enforcement at `OrganisationKeyController` and `KeyTypeController`. Test AlgemeenBeheerder access to all keytypes + VlimpersBeheerder restricted to Vlimpers-managed orgs + allowed keytype ids + out-of-scope denial. Path: `test/OrganisationRegistry.Api.IntegrationTests/Security/KeyPolicyEndpointTests.cs`.

### Implementation for US1

- [x] T016 [US1] **NO-OP** — `User` ctor (T009) already derives `Permissions` from `roles` via `RolePermissionMap.For`. `RoleMapping.Map` still needed as edge-mapping `Role↔string` for JWT emission. Kept as-is.
- [x] T017 [US1] Added `ClaimsExtension.ToPermissionSet(this ClaimsPrincipal, ILogger? = null)` — reads `ClaimTypes.Role` + raw `AcmIdmConstants.Claims.Role` (strips `RolePrefix`), filters via `RoleMapping.Exists`, maps via `RoleMapping.Map` → `RolePermissionMap.For`; reads scopes from `AcmIdmConstants.Claims.Scope` split on `' '` → `ScopePermissionMap.For`; unions.
- [x] T018 [US1] **NO-OP** — `OrganisationRegistryTokenBuilder.ParseRoles` (line 171) emits `ClaimTypes.Role` via `RoleMapping.Map`. Downstream `ToPermissionSet` reads these claims at consume-time. No source change; keeps JWT payload minimal, avoids double-computation.
- [x] T019 [US1] **NO-OP** — `TokenExchangeClaimsTransformation.AddRoleClaim` (line 91) emits `ClaimTypes.Role` via `RoleMapping.Map`. Same rationale as T018.
- [x] T020 [US1] **NO-OP** — same rationale as T018/T019. `WellknownUsers.TestClient/Cjm/Orafin` already produce correct `Permissions` via `User` ctor → `RolePermissionMap.For(roles)`. Direct scope→PermissionSet cutover would break existing `IsInAnyOf(Role.*)` sites (e.g. `SecurityService.CanUseKeyType`) before US2/US3 migrate them. Final `WellknownUser`-based scope dispatch deletion deferred to T035 (US3), which the task text already anticipates. Info-scope dispatch also deferred to US3 alongside T035.
- [x] T021 [US1] Verified. `TokenExchangeConfiguration.RequiredScopes` is declared but **never consumed** anywhere in the codebase (orphan from feature 008); not this feature's concern. `TokenExchangeClaimsTransformation` has no scope handling — path relies on introspection + role-claim emission (T019 confirmed `AddRoleClaim` at line 91 still emits `ClaimTypes.Role`). Downstream `ToPermissionSet` (T017) consumes those claims correctly. Entry point #2 semantics preserved. No source change.
- [x] T022 [P] [US1] Unit test class `PermissionMapThrottleTests` — assert (a) unknown role/scope yields empty `PermissionSet` (fail-closed), (b) 100× same unknown value logs exactly 1 Serilog `Error` event, (c) two distinct unknown values log 2 events (per-key isolation). Use `Serilog.Sinks.TestCorrelator` or in-memory `List<LogEvent>` sink. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/PermissionMapThrottleTests.cs`.
- [x] T022a [P] [US1] Unit test class `KeyPolicyTests` — verify KeyPolicy evaluation against `KeyContext` with in-scope + out-of-scope keytype ids, Vlimpers-management gates, user with unrestricted `CanManageKeys` (e.g. AlgemeenBeheerder), user with restricted grant (VlimpersBeheerder), and user without permission (denial). Path: `test/OrganisationRegistry.UnitTests/Authorization/KeyPolicyTests.cs`.

**Checkpoint**: US1 integration tests green. PermissionSet visible at all three entry points. MVP demoable: mapping table can be inspected by hitting a diagnostic endpoint or by test harness. Keys MVP end-to-end working.

---

## Phase 3.5: Keys Resource Type (Shipped MVP, Model C)

**Summary**: Keys shipped with full end-to-end restriction model. Following tasks are marked `[x]` to reflect shipped state.

- [x] T022b [US1] Create `PermissionEntry` record: `(Permission, IRestriction?)` backing `PermissionSet`. Structural equality for deduplication. Implicit conversion from bare `Permission` to unrestricted entry. Path: `src/OrganisationRegistry.Infrastructure/Authorization/PermissionEntry.cs`.
- [x] T022c [US1] Create `PermissionExtensions.RestrictedTo(Permission, IRestriction)` fluent helper for call sites. Path: `src/OrganisationRegistry.Infrastructure/Authorization/PermissionExtensions.cs`.
- [x] T022d [US1] Refactor `KeyPolicy` to use `IUser.IsSatisfiedFor(Permission, KeyContext)` evaluation. Policy constructor takes `isUnderVlimpersManagement` bool and `keyTypeIds` → builds `KeyContext` → evaluates permission + restriction in one pass. Path: `src/OrganisationRegistry/Handling/Authorization/KeyPolicy.cs`.
- [x] T022e [US1] Wire `AddOrganisationKeyCommandHandler` via `.WithKeyPolicy(command)` → pass `envelope.User` and `organisation.State.UnderVlimpersManagement + keyTypeId` to policy. Verify `KeyPolicy.Check(user).IsSuccessful`. Path: `src/OrganisationRegistry/Organisation/Keys/AddOrganisationKeyCommandHandler.cs`.
- [x] T022f [US1] Wire `UpdateOrganisationKeyCommandHandler` with same `.WithKeyPolicy(command)` pattern. Path: `src/OrganisationRegistry/Organisation/Keys/UpdateOrganisationKeyCommandHandler.cs`.
- [x] T022g [US1] Add `[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.CanManageKeys })]` to `OrganisationKeyController` (list, get, post, put, delete actions). Path: `src/OrganisationRegistry.Api/Backoffice/Organisation/Key/OrganisationKeyController.cs`.
- [x] T022h [US1] Add `[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.CanManageKeys })]` to `KeyTypeController` (list, get, post, put, delete actions). Path: `src/OrganisationRegistry.Api/Backoffice/Parameters/KeyType/KeyTypeController.cs`.

---

## Phase 4: User Story 2 — Controllers checken enkel algemene permissies (P2)

**Goal**: Every controller action uses `[OrganisationRegistryAuthorize(RequiredPermissions = new[] { … })]`; no rolname / scope-string references remain in controller layer. Elke rol (incl. `AlgemeenBeheerder`) krijgt granulair alle benodigde permissions via `RolePermissionMap` — geen admin-bypass in de attribute.

**Independent Test**: Static scan of controllers shows zero references to `Role.*` or `AcmIdmConstants.Scopes.*`; every authorized action carries `RequiredPermissions`; identities with matching permission pass, others get 403.

### Tests for US2

- [x] T023 [P] [US2] Integration test class `ControllerPermissionEnforcementTests` — parametrized over a representative sample of endpoints (one per permission), asserting 200 vs 403 based on identity's PermissionSet. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/Security/ControllerPermissionEnforcementTests.cs`.
- [ ] T024 [P] [US2] Unit test class `OrganisationRegistryAuthorizeAttributeTests` — attribute admits identity when PermissionSet contains any of `RequiredPermissions`; identity without required permission gets 403. Geen admin-short-circuit. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/OrganisationRegistryAuthorizeAttributeTests.cs`.

### Implementation for US2

- [ ] T025 [US2] Extend `OrganisationRegistryAuthorizeAttribute` with `Permission[] RequiredPermissions { get; set; }`. In `OnAuthorization`, after resolving `IUser`: if any of `RequiredPermissions` in `Permissions` → allow; else → `403`. Parameterloos gebruik (geen `RequiredPermissions`) valt terug op enkel policy-checks (`BackofficeUser`). Keep legacy `Roles`-based path only during migration if it exists, but mark obsolete. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/Security/OrganisationRegistryAuthorizeAttribute.cs`.
- [ ] T026 [US2] **Controller sweep — split at execution time into T026a/b/c per controller folder** (Backoffice, Search, Integration/other) for reviewability. Sweep all controllers under `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/`: replace every `[OrganisationRegistryAuthorize(Roles = …)]` (or equivalent role-based check) with `[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.X })]`. Use ast-grep to find all attribute usages; map each per the endpoint's business intent (documented case-by-case in PR description). One commit per folder.
- [ ] T027 [US2] Remove all direct `IUser.Roles.Contains(...)` / role-string comparisons / `AcmIdmConstants.Scopes.*` comparisons from controller code. Replace with `IUser.HasPermission(...)` when the check must remain, or delete when moving to attribute.
- [ ] T028 [US2] Modify `PolicyNames.cs` (if it enumerates authorization policies) to align with permission ids; delete obsolete role-based policy names. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/PolicyNames.cs`.

**Checkpoint**: `grep -R "Role\." src/OrganisationRegistry.Api/` returns only edge-translation files (RoleMapping, ClaimsExtension, TokenBuilder, TokenExchange). Controller sweep clean.

---

## Phase 5: User Story 3 — Policies checken enkel restricties/scope (P3)

**Goal**: All resource-type policies remove role/scope-string checks; they gate on resource-level scope only via `IRestrictionContext` evaluation. Note: **ONLY KeyPolicy shipped (MVP)**. All other policies (Label, Vlimpers, Orgaan, Regelgeving) remain DEFERRED to future PRs following the same restriction pattern.

**Deferred resources** (NOT in MVP): Labels, Capacities, FormalFrameworks, OrganisationClassifications, Regulations, Import, Delegations, Bodies. These will follow the Keys pattern in future PRs.

**Independent Test**: KeyPolicy (shipped) verified by T015a, T022a. Other policies deferred.

### Tests for US3 (Deferred)

- [ ] T029 [P] [US3] Unit test class `LabelPolicyRestrictionTests` — restriction context for label operations (org id, label type). Deferred. Path: `test/OrganisationRegistry.UnitTests/Authorization/LabelPolicyRestrictionTests.cs`.
- [ ] T030 [P] [US3] Unit test class `VlimpersPolicyRestrictionTests` — restriction context for Vlimpers operations. Deferred. Path: `test/OrganisationRegistry.UnitTests/Authorization/VlimpersPolicyRestrictionTests.cs`.

### Implementation for US3 (Deferred)

- [ ] T031 [US3] Create policy contexts for non-key resource types (e.g. `LabelContext`, `VlimpersContext`). Deferred.
- [ ] T032 [US3] Refactor label policies (`LabelPolicy`, etc.) to use `IRestrictionContext` evaluation. Deferred.
- [ ] T033 [US3] Refactor Vlimpers policies (`VlimpersPolicy`, `VlimpersOnlyPolicy`, `BeheerderForOrganisationRegardlessOfVlimpersPolicy`) to use restriction evaluation. Deferred.
- [ ] T034 [US3] Refactor remaining 13 policies. Deferred.
- [ ] T035 [US3] Delete `WellknownUser`-based scope→user dispatch entirely from `SecurityService` (already touched in T020); confirm no residual references. Deferred (depends on all policies shipped).

**Checkpoint**: Deferred to future PRs. KeyPolicy complete and tested.

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

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: no deps.
- **Foundational (Phase 2)**: after Setup. Blocks US1/US2/US3.
- **US1 (Phase 3)**: after Foundational. MVP shipped complete; all tasks marked `[x]`.
- **US1 Keys MVP (Phase 3.5)**: after US1, contained within c3b0d4af5. All tasks marked `[x]`.
- **US2 (Phase 4)**: after US1 complete. Pending (DEFERRED after MVP shipped).
- **US3 (Phase 5)**: after US2. Pending (DEFERRED; only KeyPolicy shipped in MVP).
- **Polish (Phase 6)**: after all user stories complete. Partially complete; see T036–T042 status.

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
4. ⏸ US2 → controllers reduced to permission checks (deferred; Keys done, others TBD).
5. ⏸ US3 → policies reduced to scope-only (deferred; only KeyPolicy shipped).
6. ⏸ Polish → cleanup + docs (partial; T036 WON'T DO, T037 NO-OP, T038–T042 partial/deferred).

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
