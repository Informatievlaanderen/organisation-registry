# Data Model: Permission-Based Authorization

Infrastructure types only. No aggregates, no events, no persisted state — all types are runtime request-scoped or startup-static.

## Types

### `Permission` (enum, `OrganisationRegistry.Infrastructure.Authorization`)

Closed set of technical capability identifiers. PascalCase C# enum members; identifiers referenced identically in docs and UI (no camelCase variants).

```
CanEditChildren
CanEditVlimpers
CanEditDelegations
CanAddLocations
CanAddBodies
CanEditBodies
CanRegisterBodies
CanManageKeys
CanManageLabels
CanManageCapacities
CanManageFormalFrameworks
CanManageOrganisationClassifications
CanManageRegulations
CanImport
CanRunScheduledJobs
CanReadOrafin
CanReadInfoEndpoints                  // NEW — granted only to Info scope
```

(Final list ratified during Phase 2 task generation after full policy sweep.)

### `PermissionSet` (immutable value object)

- Backed by `IReadOnlySet<Permission>`.
- Constructors: `PermissionSet.Empty`, `PermissionSet.Of(params Permission[])`.
- Operations: `Contains(Permission)`, `Union(PermissionSet)` — returns new set (immutable).
- Equality: set-equality.

### `RolePermissionMap` (static)

- `IReadOnlyDictionary<Role, PermissionSet>` populated at startup.
- `PermissionSet For(Role role)` — returns `PermissionSet.Empty` + logs error for unmapped roles.
- `PermissionSet For(IEnumerable<Role> roles)` — folds via `Union`.

### `ScopePermissionMap` (static, NEW)

- `IReadOnlyDictionary<string, PermissionSet>` populated at startup, keyed by literal scope string (e.g. `"dv_organisatieregister_orafinbeheerder"`).
- `PermissionSet For(string scope)` — returns `PermissionSet.Empty` + logs error for unmapped scopes.
- `PermissionSet For(IEnumerable<string> scopes)` — folds via `Union`; accepts the space-split output of the `scope` claim.

Referenced constants live in `AcmIdmConstants.Scopes`.

### `IUserRestrictionsProvider` (interface, NEW — DI-scoped)

Resource-level scope restrictions fetched just-in-time from SQL Server projections; memoised for the lifetime of a single HTTP request.

```csharp
public interface IUserRestrictionsProvider
{
    Task<IReadOnlySet<Guid>> GetOrganisationIdsAsync(CancellationToken ct);
    Task<IReadOnlySet<string>> GetOvoNumbersAsync(CancellationToken ct);
    Task<IReadOnlySet<Guid>> GetBodyIdsAsync(CancellationToken ct);
    Task<IReadOnlySet<Guid>> GetRegulationIdsAsync(CancellationToken ct);

    Task<bool> IsInScopeForOrganisationAsync(Guid organisationId, CancellationToken ct);
    Task<bool> IsInScopeForOrganisationAsync(string ovoNumber, CancellationToken ct);
    Task<bool> IsInScopeForBodyAsync(Guid bodyId, CancellationToken ct);
}
```

Implementation binds to the current `IUser` (via DI), queries SQL Server read-models, caches results per (user, resource-type) for the request lifetime.

### `IUser` extension

Add:
```
bool HasPermission(Permission permission);
PermissionSet Permissions { get; }
```

Remove (post-cutover):
```
Role[] Roles { get; set; }
bool IsInAnyOf(params Role[] roles);
bool IsAuthorizedForVlimpersOrganisations { get; }
bool IsDecentraalBeheerderForOrganisation(string ovoNumber);
bool IsDecentraalBeheerderForOrganisation(Guid organisationId);
bool IsDecentraalBeheerderForBody(Guid bodyId);
IReadOnlyList<string> Organisations { get; }
IReadOnlyList<Guid> OrganisationIds { get; }
IReadOnlyList<Guid> Bodies { get; }
```

Resource-restriction collections migrate to `IUserRestrictionsProvider` (JIT).

### `User` constructor change

Takes `PermissionSet permissions` instead of `Role[] roles`. Entry points call:

```csharp
var permissions = RolePermissionMap
    .For(roles)
    .Union(ScopePermissionMap.For(scopes));
var user = new User(..., permissions);
```

## Invariants

- `PermissionSet` is immutable — never mutated after construction.
- `RolePermissionMap` and `ScopePermissionMap` are populated exactly once, at type initialization.
- `User.Permissions` is non-null (defaults to `PermissionSet.Empty`).
- Unknown role or scope → empty set + Serilog error (fail-closed).
- `IUserRestrictionsProvider` results are cached per request; never persisted across requests.
