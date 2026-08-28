# Tasks: Permission-Based Authorization

**Input**: Design documents from `/code/aiv/organisation-registry/specs/009-permission-based-authz/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/permission-check-api.md, quickstart.md

**Tests**: Included — feature spec explicitly requires unit + integration + policy tests (Constitution Principle V, plan §Testing).

**Organization**: Tasks grouped by user story. US1 = MVP.

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

- [x] T004 [P] Create `Permission` enum with all identifiers in PascalCase (`CanEditAll, CanEditChildren, CanEditVlimpers, CanEditDelegations, CanAddLocations, CanAddBodies, CanEditBodies, CanRegisterBodies, CanManageKeys, CanManageLabels, CanManageCapacities, CanManageFormalFrameworks, CanManageOrganisationClassifications, CanManageRegulations, CanImport, CanRunScheduledJobs, CanReadOrafin, CanReadInfoEndpoints`). Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/Permission.cs`.
- [x] T005 [P] Create immutable `PermissionSet` value object (backed by `ImmutableHashSet<Permission>`) with `Empty`, `Union`, `Contains`, `ContainsAny`, `ContainsAll`. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/PermissionSet.cs`.
- [x] T006 [P] Create `IUserRestrictionsProvider` interface exposing `GetOrganisationScope(IUser)`, `GetBodyScope(IUser)`, `GetRegulationScope(IUser)` returning cached `IReadOnlyCollection<Guid>` per resource type per request. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/IUserRestrictionsProvider.cs`.
- [x] T007 Create `RolePermissionMap` static class exposing `For(IEnumerable<Role>) : PermissionSet`. Encode the full mapping for `AlgemeenBeheerder, VlimpersBeheerder, DecentraalBeheerder, RegelgevingBeheerder, OrgaanBeheerder, Developer, CjmBeheerder, Orafin` per data-model.md. Fail-closed + Serilog error (throttled once/role/process) for unknown roles. `AlgemeenBeheerder` MUST include `CanEditAll`. Do NOT map `AutomatedTask`. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/RolePermissionMap.cs`.
- [x] T008 Create `ScopePermissionMap` static class exposing `For(IEnumerable<string>) : PermissionSet` keyed by exact scope strings from `AcmIdmConstants.Scopes` (`dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_orafinbeheerder`, `dv_organisatieregister_info`, `dv_organisatieregister_testclient`). `Info` → `CanReadInfoEndpoints` only. Fail-closed + Serilog error (throttled once/scope/process). Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/ScopePermissionMap.cs`.
- [x] T009 Modify `User` to carry `PermissionSet Permissions` (constructor arg) plus `HasPermission(Permission)` and `HasAnyPermission(params Permission[])`. Keep existing `Roles` property temporarily on the object surface but remove all internal reads (mark `[Obsolete("Roles are edge-only; use Permissions")]` — remove in polish). Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/User.cs`.
- [x] T010 [P] Unit tests: `PermissionSetTests` (union, contains-any/all, empty). Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/PermissionSetTests.cs`.
- [x] T011 [P] Unit tests: `RolePermissionMapTests` — one theory row per role verifying exact PermissionSet + fail-closed on unknown. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/RolePermissionMapTests.cs`.
- [x] T012 [P] Unit tests: `ScopePermissionMapTests` — one theory row per scope + fail-closed + `Info`-scope isolation. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/ScopePermissionMapTests.cs`.

**Checkpoint**: `dotnet build` green; foundational unit tests green. US1/US2/US3 unblocked.

---

## Phase 3: User Story 1 — Rollen én scopes vertalen naar permissies aan de systeemrand (P1) 🎯 MVP

**Goal**: All three entry points (edit-api, token-exchange, bearer/CC scope) produce an `IUser` whose `Permissions` is built via `RolePermissionMap ∪ ScopePermissionMap`. Internally no role or scope-string is read anymore.

**Independent Test**: For each role and each CC scope, authenticate via the matching entry point and assert the resulting PermissionSet equals the documented mapping (via integration test harness).

### Tests for US1 (write first, expect fail)

- [x] T013 [P] [US1] Integration test `EditApiPermissionTranslationTests` — baseline regression (option B): interactive user JWT via `ApiFixture.HttpClient` → `GET /v1/security` → 200 + `AlgemeenBeheerder` role. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/Security/EditApiPermissionTranslationTests.cs`.
- [x] T014 [P] [US1] Integration test `TokenExchangePermissionTranslationTests` — baseline regression (option B): Keycloak CC token for CJM/Orafin via token-exchange helper → `GET /v1/security` → 200 + expected role. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/Security/TokenExchangePermissionTranslationTests.cs`.
- [x] T015 [P] [US1] Integration test `ClientCredentialsScopePermissionTests` — baseline regression (option B): direct bearer for Test/CJM/Orafin CC clients → `GET /v1/security` → 200 + expected role. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/Security/ClientCredentialsScopePermissionTests.cs`.

### Implementation for US1

- [x] T016 [US1] **NO-OP** — `User` ctor (T009) already derives `Permissions` from `roles` via `RolePermissionMap.For`. `RoleMapping.Map` still needed as edge-mapping `Role↔string` for JWT emission. Kept as-is.
- [x] T017 [US1] Added `ClaimsExtension.ToPermissionSet(this ClaimsPrincipal, ILogger? = null)` — reads `ClaimTypes.Role` + raw `AcmIdmConstants.Claims.Role` (strips `RolePrefix`), filters via `RoleMapping.Exists`, maps via `RoleMapping.Map` → `RolePermissionMap.For`; reads scopes from `AcmIdmConstants.Claims.Scope` split on `' '` → `ScopePermissionMap.For`; unions.
- [x] T018 [US1] **NO-OP** — `OrganisationRegistryTokenBuilder.ParseRoles` (line 171) emits `ClaimTypes.Role` via `RoleMapping.Map`. Downstream `ToPermissionSet` reads these claims at consume-time. No source change; keeps JWT payload minimal, avoids double-computation.
- [x] T019 [US1] **NO-OP** — `TokenExchangeClaimsTransformation.AddRoleClaim` (line 91) emits `ClaimTypes.Role` via `RoleMapping.Map`. Same rationale as T018.
- [x] T020 [US1] **NO-OP** — same rationale as T018/T019. `WellknownUsers.TestClient/Cjm/Orafin` already produce correct `Permissions` via `User` ctor → `RolePermissionMap.For(roles)`. Direct scope→PermissionSet cutover would break existing `IsInAnyOf(Role.*)` sites (e.g. `SecurityService.CanUseKeyType`) before US2/US3 migrate them. Final `WellknownUser`-based scope dispatch deletion deferred to T035 (US3), which the task text already anticipates. Info-scope dispatch also deferred to US3 alongside T035.
- [x] T021 [US1] Verified. `TokenExchangeConfiguration.RequiredScopes` is declared but **never consumed** anywhere in the codebase (orphan from feature 008); not this feature's concern. `TokenExchangeClaimsTransformation` has no scope handling — path relies on introspection + role-claim emission (T019 confirmed `AddRoleClaim` at line 91 still emits `ClaimTypes.Role`). Downstream `ToPermissionSet` (T017) consumes those claims correctly. Entry point #2 semantics preserved. No source change.
- [x] T022 [P] [US1] Unit test class `PermissionMapThrottleTests` — assert (a) unknown role/scope yields empty `PermissionSet` (fail-closed), (b) 100× same unknown value logs exactly 1 Serilog `Error` event, (c) two distinct unknown values log 2 events (per-key isolation). Use `Serilog.Sinks.TestCorrelator` or in-memory `List<LogEvent>` sink. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/PermissionMapThrottleTests.cs`.

**Checkpoint**: US1 integration tests green. PermissionSet visible at all three entry points. MVP demoable: mapping table can be inspected by hitting a diagnostic endpoint or by test harness.

---

## Phase 4: User Story 2 — Controllers checken enkel algemene permissies (P2)

**Goal**: Every controller action uses `[OrganisationRegistryAuthorize(RequiredPermissions = new[] { … })]`; no rolname / scope-string references remain in controller layer. `CanEditAll` always satisfies.

**Independent Test**: Static scan of controllers shows zero references to `Role.*` or `AcmIdmConstants.Scopes.*`; every authorized action carries `RequiredPermissions`; identities with matching permission pass, others get 403.

### Tests for US2

- [x] T023 [P] [US2] Integration test class `ControllerPermissionEnforcementTests` — parametrized over a representative sample of endpoints (one per permission), asserting 200 vs 403 based on identity's PermissionSet. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/Security/ControllerPermissionEnforcementTests.cs`.
- [ ] T024 [P] [US2] Unit test class `OrganisationRegistryAuthorizeAttributeTests` — attribute admits identity when PermissionSet contains any of `RequiredPermissions`; `CanEditAll` short-circuits to allow. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/OrganisationRegistryAuthorizeAttributeTests.cs`.

### Implementation for US2

- [ ] T025 [US2] Extend `OrganisationRegistryAuthorizeAttribute` with `Permission[] RequiredPermissions { get; set; }`. In `OnAuthorization`, after resolving `IUser`: if `Permissions.Contains(CanEditAll)` → allow; else if any of `RequiredPermissions` in `Permissions` → allow; else → `403`. Keep legacy `Roles`-based path only during migration if it exists, but mark obsolete. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/Security/OrganisationRegistryAuthorizeAttribute.cs`.
- [ ] T026 [US2] **Controller sweep — split at execution time into T026a/b/c per controller folder** (Backoffice, Search, Integration/other) for reviewability. Sweep all controllers under `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/`: replace every `[OrganisationRegistryAuthorize(Roles = …)]` (or equivalent role-based check) with `[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.X })]`. Use ast-grep to find all attribute usages; map each per the endpoint's business intent (documented case-by-case in PR description). One commit per folder.
- [ ] T027 [US2] Remove all direct `IUser.Roles.Contains(...)` / role-string comparisons / `AcmIdmConstants.Scopes.*` comparisons from controller code. Replace with `IUser.HasPermission(...)` when the check must remain, or delete when moving to attribute.
- [ ] T028 [US2] Modify `PolicyNames.cs` (if it enumerates authorization policies) to align with permission ids; delete obsolete role-based policy names. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/PolicyNames.cs`.

**Checkpoint**: `grep -R "Role\." src/OrganisationRegistry.Api/` returns only edge-translation files (RoleMapping, ClaimsExtension, TokenBuilder, TokenExchange). Controller sweep clean.

---

## Phase 5: User Story 3 — Policies checken enkel restricties/scope (P3)

**Goal**: All 17 `ISecurityPolicy` implementations remove role/scope-string checks; they gate on resource-level scope only. Restrictions fetched via `IUserRestrictionsProvider` (JIT, request-scoped).

**Independent Test**: For every policy, verify (a) no `Role.*` or scope-string reads remain, (b) scope restriction fires when identity is out-of-scope, (c) identity with matching permission + in-scope resource is admitted.

### Tests for US3

- [ ] T029 [P] [US3] Unit test class `UserRestrictionsProviderTests` — verify SQL projection is called once per resource-type per request; second call served from memoisation; different `IUser` yields fresh fetch. Uses in-memory fake for the projection queryable. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/UserRestrictionsProviderTests.cs`.
- [ ] T030 [P] [US3] Unit test class `PolicyScopeTests` — one theory row per `ISecurityPolicy`: identity in scope → allow, identity out of scope → deny, with mocked `IUserRestrictionsProvider`. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.UnitTests/Authorization/PolicyScopeTests.cs`.
- [ ] T031 [P] [US3] **Pre-check**: inspect `test/OrganisationRegistry.SqlServer.IntegrationTests/` and `test/OrganisationRegistry.Api.IntegrationTests/` for existing SQL test harness pattern (Testcontainers vs. shared LocalDB fixture). Reuse — do NOT introduce a new harness. Then write integration test `JitRestrictionFetchTests` asserting that a policy evaluation for an in-scope org uses live projection data and out-of-scope org is refused. Path: `/code/aiv/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/Security/JitRestrictionFetchTests.cs`.

### Implementation for US3

- [ ] T032 [US3] Implement `UserRestrictionsProvider` (concrete): reads from existing SQL Server projections (`OrganisationRegistryContext`) — organisation-tree projection for DecentraalBeheerder scope, Vlimpers projection, Orgaan-membership projection, Regelgeving projection. Request-scoped memoisation via injected `IHttpContextAccessor` + per-instance dictionary (scoped lifetime is sufficient — no static state). Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/Security/UserRestrictionsProvider.cs`.
- [ ] T033 [US3] Register `IUserRestrictionsProvider` as scoped in DI composition root. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Api/Startup.cs` (or the equivalent DI module).
- [ ] T034 [US3] Refactor all 17 policies under `/code/aiv/organisation-registry/src/OrganisationRegistry/Handling/Authorization/`: remove `IUser.Roles.Contains(...)` reads and any `AcmIdmConstants` reads; inject/consume `IUserRestrictionsProvider` for resource scope; assume permission gate already passed at controller. Each policy = one commit for reviewability.
- [ ] T035 [US3] Delete `WellknownUser`-based scope→user dispatch entirely from `SecurityService` (already touched in T020); confirm no residual references. Grep-based validation.

**Checkpoint**: All 17 policies pass unit + integration tests. `grep -R "Roles.Contains" src/OrganisationRegistry/` returns zero hits.

---

## Phase 6: Polish & Cross-Cutting

- [ ] T036 [P] **Pre-check**: grep event store, projections, and event classes for `AutomatedTask` usage — event sourcing means historical events are immutable, so if `Role.AutomatedTask` appears in any persisted event payload (e.g. `RoleAssigned`), the enum value MUST stay for deserialization even if no new code writes it. Confirm zero historical usage before deleting the enum value; otherwise mark `[Obsolete]` and keep. Then delete `AutomatedTask` role from `Role.cs` enum (if safe); remove all references; ensure the codebase compiles. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/Role.cs`.
- [ ] T037 [P] Delete `[Obsolete]` `Roles` surface from `IUser`/`User` once all consumers are migrated; verify by build. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/User.cs`.
- [ ] T038 [P] Update `AGENTS.md` recent-changes section with permission-model summary (single-line addition; automated by `.specify/scripts/bash/update-agent-context.sh opencode`). Path: `/code/aiv/organisation-registry/AGENTS.md`.
- [ ] T039 [P] Add centralized permission catalog doc referencing spec + data-model. Path: `/code/aiv/organisation-registry/src/OrganisationRegistry.Infrastructure/Authorization/README.md`.
- [ ] T040 Run quickstart.md validation: authenticate via all three entry points against a local dev instance; confirm PermissionSet + JIT restriction fetch behavior. Path: `/code/aiv/organisation-registry/specs/009-permission-based-authz/quickstart.md`.
- [ ] T041 Static analysis pass: assert `grep -R "AcmIdmConstants.Scopes" src/` shows only `ScopePermissionMap.cs`; `grep -R "Role\." src/OrganisationRegistry.Api/` shows only edge files; success criterion SC-006 met.
- [ ] T042 Run full test suite (`dotnet test`) + verify no regression in existing authorization tests.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: no deps.
- **Foundational (Phase 2)**: after Setup. Blocks US1/US2/US3.
- **US1 (Phase 3)**: after Foundational. Independent thereafter.
- **US2 (Phase 4)**: after US1 complete. **Strict serial order US1 → US2 → US3** (revised from earlier parallel-scaffolding suggestion) — US3's policy refactor depends on final attribute-layer decisions made in US2.
- **US3 (Phase 5)**: after US2 complete.
- **Polish (Phase 6)**: after all user stories complete.

### Story-level Dependencies

- US2 (controllers) depends on US1 (PermissionSets exist to check).
- US3 (policies) depends on US1 (identities have PermissionSets) but is orthogonal to US2 (attribute layer).

### Parallel Opportunities

- Foundational: T004, T005, T006 fully parallel; T010, T011, T012 parallel (after respective impl).
- US1: T013, T014, T015 parallel (different test files). T018, T019, T020 touch different entry-point files → parallel.
- US2: T023 and T024 parallel. Controller sweep T026 is sequential per file family but a single agent can chunk it.
- US3: T029, T030, T031 parallel test authorship. Policy refactor T034 is 17 files → parallelisable by folder/module.
- Polish: T036, T037, T038, T039 parallel.

---

## Parallel Example

```bash
# Foundational parallel batch (after T001–T003):
Task: "Create Permission enum"                        # T004
Task: "Create PermissionSet value object"             # T005
Task: "Create IUserRestrictionsProvider interface"    # T006

# US1 test-first parallel batch:
Task: "Write EditApiPermissionTranslationTests"       # T013
Task: "Write TokenExchangePermissionTranslationTests" # T014
Task: "Write ClientCredentialsScopePermissionTests"   # T015
```

---

## Implementation Strategy

### MVP (US1 only)

1. Phase 1 → Phase 2 → Phase 3.
2. Validate: run US1 tests; inspect diagnostic endpoint (or test harness output) for PermissionSet at each entry point.
3. Merge behind feature-flag OR keep dual-check off — cutover release requires US2 + US3 too, so MVP is internal validation only.

### Incremental Delivery (recommended)

1. Setup + Foundational → foundation ready.
2. US1 → PermissionSets flowing at edges.
3. US2 → controllers reduced to permission checks.
4. US3 → policies reduced to scope-only.
5. Polish → cleanup + docs + `AutomatedTask` deletion.

### Parallel Team Strategy

- After Foundational: Dev A on US1, Dev B on US2 skeleton (attribute + tests), Dev C on US3 test scaffolding + `UserRestrictionsProvider` implementation. Merge order: US1 → US2 → US3.

---

## Notes

- Cutover release: US1+US2+US3 must ship together to avoid a partial state where edges emit PermissionSets but downstream still reads roles.
- No new NuGet packages required.
- No event-store changes.
- Domain naming Dutch preserved; permission ids PascalCase English.
- Fail-closed enforced at both maps; Serilog throttled to prevent log flooding.
- SC-006 acceptance: only `RolePermissionMap.cs` + `ScopePermissionMap.cs` remain as internal role/scope readers.
