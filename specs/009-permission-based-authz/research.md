# Phase 0 Research: Permission-Based Authorization

## 1. Current State Analysis

### Roles (source: `src/OrganisationRegistry.Infrastructure/Authorization/Role.cs`)
Enum with 9 values: `AlgemeenBeheerder`, `VlimpersBeheerder`, `DecentraalBeheerder`, `OrgaanBeheerder`, `RegelgevingBeheerder`, `Orafin`, `CjmBeheerder`, `Developer`, `AutomatedTask`.

`AutomatedTask` is deprecated by this feature — automated processes enter via ACM/IDM Client Credentials with scopes, never with a role.

### Scopes (source: `src/OrganisationRegistry.Infrastructure/Authorization/AcmIdmConstants.cs`)
Client-Credentials bearer tokens carry space-separated scope strings in the `AcmIdmConstants.Claims.Scope` claim. Full catalog:

| Constant | Scope string |
|---|---|
| `Scopes.CjmBeheerder` | `dv_organisatieregister_cjmbeheerder` |
| `Scopes.OrafinBeheerder` | `dv_organisatieregister_orafinbeheerder` |
| `Scopes.Info` | `dv_organisatieregister_info` |
| `Scopes.TestClient` | `dv_organisatieregister_testclient` |

No vlimpers scope exists — vlimpers capabilities are role-only.

### Entry Points (claim → IUser translation sites)
1. **Edit-API endpoint** — `OrganisationRegistryTokenBuilder.cs` (builds token/User for edit sessions from role claims).
2. **Token exchange** — `TokenExchangeClaimsTransformation.cs` (ACM/IDM role claim → internal claims); scoped by `TokenExchangeConfiguration.RequiredScopes`.
3. **Bearer token / Client Credentials** — `SecurityService.GetRequiredUser` (lines 146–164): extracts the `scope` claim, splits on space, currently dispatches to `WellknownUsers.*`. This is the exact site that must be replaced by `ScopePermissionMap`-driven translation.

All three ultimately produce a `User` instance (`src/OrganisationRegistry.Infrastructure/Authorization/User.cs`) which today carries `Role[] Roles`.

### Downstream Consumers
- **~17 `ISecurityPolicy` implementations** in `src/OrganisationRegistry/Handling/Authorization/` — call `user.IsInAnyOf(Role.X)` and/or scope predicates (`IsDecentraalBeheerderForOrganisation(...)`).
- **`OrganisationRegistryAuthorizeAttribute`** — controller-level guard, currently role-based.
- **`User.IsAuthorizedForVlimpersOrganisations`** — role-based property.
- **Scope helpers** — mix role + membership check.

## 2. Role → Permission Mapping

Derived from observed usage in policies. Permission ids are PascalCase C# enum members; role/scope names stay Dutch.

| Role | Permissions | Scope Restriction |
|---|---|---|
| `AlgemeenBeheerder` | `CanEditChildren`, `CanAddBodies`, `CanEditBodies`, `CanRegisterBodies`, `CanAddLocations`, `CanManageKeys`, `CanManageLabels`, `CanManageCapacities`, `CanManageFormalFrameworks`, `CanManageOrganisationClassifications`, `CanManageRegulations`, `CanImport`, `CanEditVlimpers`, `CanEditDelegations` | none (global) |
| `VlimpersBeheerder` | `CanEditVlimpers`, `CanEditChildren` (vlimpers-scoped) | vlimpers organisations only |
| `DecentraalBeheerder` | `CanEditChildren`, `CanAddLocations`, `CanManageKeys`, `CanAddBodies`, `CanEditBodies` | own organisations / bodies (JIT) |
| `OrgaanBeheerder` | `CanAddBodies`, `CanEditBodies`, `CanRegisterBodies` | none (global for body ops) |
| `RegelgevingBeheerder` | `CanManageRegulations` | own regulations (JIT) |
| `CjmBeheerder` | `CanAddBodies`, `CanEditBodies` | CJM-scope |
| `Orafin` | `CanReadOrafin` | none |
| `Developer` | superset (equivalent to `AlgemeenBeheerder` + vlimpers) | none |
| ~~`AutomatedTask`~~ | **removed** — use CC scope instead | — |

**Decision**: `AlgemeenBeheerder` is granted each capability permission explicitly (no admin-bypass short-circuit). Any future admin-bypass model is out of scope for this feature.

**Unknown role**: fail-closed — empty `PermissionSet`, Serilog error `Unmapped role: {Role}` (once per role per process).

**Multi-role**: `PermissionSet.Union(other)` — widest scope wins.

## 3. Scope → Permission Mapping

Client Credentials bearer tokens translate at the same layer as roles, via an analogous static `ScopePermissionMap`.

| Scope | Permissions | Scope Restriction |
|---|---|---|
| `dv_organisatieregister_cjmbeheerder` | `CanAddBodies`, `CanEditBodies` | none (CC has no user context) |
| `dv_organisatieregister_orafinbeheerder` | `CanReadOrafin` | none |
| `dv_organisatieregister_info` | `CanReadInfoEndpoints` (new dedicated permission) | none |
| `dv_organisatieregister_testclient` | (test-only, TBD in Phase 2 task sweep) | none |

**Vlimpers is role-only** — no scope grants vlimpers permissions.

**Unknown scope**: fail-closed — empty `PermissionSet`, Serilog error `Unmapped scope: {Scope}`.

**Multi-scope**: `PermissionSet.Union(other)` across all scopes in the space-separated claim.

**Combined**: When a request carries both roles and scopes (rare, but possible), the effective set is `RolePermissionMap.For(roles).Union(ScopePermissionMap.For(scopes))`.

## 4. Architectural Options Considered

### Option A: `Permission` as `enum` — **CHOSEN**
- **Pros**: compile-time safety, `HashSet<Permission>` is fast, easy pattern-match.
- **Cons**: adding a permission requires enum edit + recompile.
- **Verdict**: closed set of ~18 permissions, low churn, safety wins.

### Option B: `Permission` as string-typed value object
- **Pros**: extensible without recompile, easy to serialise.
- **Cons**: typo risk, less IDE support.
- **Verdict**: rejected for internal type.

### Option C: Keep `Role[]` internally, add `HasPermission` as computed method
- **Pros**: smallest diff.
- **Cons**: violates cutover goal — roles still leak into `IUser` surface.
- **Verdict**: rejected.

## 5. Scope Restriction Handling (Resource-Level)

Users may hold restrictions for tens to ~100 organisations. Caching all in `IUser` bloats every request; recomputing on each policy call is expensive. Chosen strategy:

- **Just-in-time fetch** at policy evaluation via `IUserRestrictionsProvider`.
- **Request-scoped memoisation** — DI-scoped instance; first fetch per (user, resource-type) hits SQL Server projections, subsequent calls in the same request return the cached result.
- **Data source**: existing SQL Server projections (read-models). NOT ElasticSearch, NOT event-replay.
- **`IUser` no longer carries** `Organisations`, `OrganisationIds`, `Bodies`, `Regulations` restriction collections.

Policies become:
1. Permission gate (`user.HasPermission(...)`) — general capability check.
2. Scope gate (`restrictions.IsInScopeForOrganisation(orgId)`, etc.) — resource-level check via provider.

Controllers use only permission gates via `[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.X })]`.

## 6. Cache & Lifecycle

- `RolePermissionMap` and `ScopePermissionMap` are static readonly dictionaries populated at startup — no invalidation.
- `User.Permissions` is per-request, constructed at translation — no cache to invalidate.
- `IUserRestrictionsProvider` is DI-scoped — lifetime = HTTP request; disposed with request scope.

## 7. Testing Strategy

- **Unit** (`test/OrganisationRegistry.UnitTests/Authorization/`):
  - `RolePermissionMapTests` — every role maps to expected permission set; unknown role → empty + error log.
  - `ScopePermissionMapTests` — every scope maps to expected permission set; unknown scope → empty + error log; multi-scope union.
  - `PermissionSetTests` — union semantics, immutability, equality.
  - `UserRestrictionsProviderTests` — JIT fetch, request-scoped memoisation, projection integration.
  - Policy tests refactored to inject `IUser` with permission set + mock `IUserRestrictionsProvider`.
- **Integration** (`test/OrganisationRegistry.Api.IntegrationTests/Security/`):
  - Per entry point (edit-api, token-exchange, bearer/CC): given ACM/IDM claim set, resulting `User` has correct permissions.
  - End-to-end: request with `DecentraalBeheerder` role → allowed on own org (JIT restrictions fetched), forbidden on other.
  - End-to-end: CC bearer with `dv_organisatieregister_orafinbeheerder` scope → `CanReadOrafin` granted.

## 8. Migration Path (Cutover)

1. Introduce `Permission`, `PermissionSet`, `RolePermissionMap`, `ScopePermissionMap`, `IUserRestrictionsProvider`, `IUser.HasPermission` alongside existing `Role[]`.
2. Refactor all 17 policies + attribute + 3 entry points in a single cutover PR.
3. Remove `IsInAnyOf`, `Role[] Roles`, `IsAuthorizedForVlimpersOrganisations`, resource-restriction collections from `IUser`/`User`.
4. Delete `Role.AutomatedTask`; delete `WellknownUsers.*` dispatch in `SecurityService`.
5. `Role` enum retained only for entry-point translation; `internal` to `Infrastructure.Authorization` + API security namespace.

## 9. Open Questions

None. All 5 clarifications resolved (see spec.md §Clarifications).
