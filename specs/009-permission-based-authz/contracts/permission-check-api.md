# Contract: Permission Check API (Internal)

Internal boundary between authorization plumbing and consumers (controllers, policies). No HTTP surface.

## `IUser.HasPermission(Permission)`

**Signature**: `bool HasPermission(Permission permission)`

**Contract**:
- Returns `true` iff `permission` is contained in `Permissions`.
- Pure, side-effect-free, O(1).
- Never throws.

## `IUser.Permissions`

**Signature**: `PermissionSet Permissions { get; }`

**Contract**:
- Non-null; returns `PermissionSet.Empty` for anonymous/unmapped users.
- Immutable reference — safe to hold, safe to share.

## `RolePermissionMap.For(Role)`

**Signature**: `PermissionSet For(Role role)`

**Contract**:
- Returns the configured `PermissionSet` for `role`.
- Unmapped `Role` values: returns `PermissionSet.Empty` and logs `Serilog.Log.Error("Unmapped role: {Role}", role)` exactly once per role per process (throttled).

## `RolePermissionMap.For(IEnumerable<Role>)`

**Signature**: `PermissionSet For(IEnumerable<Role> roles)`

**Contract**:
- Returns the union of permission sets for all roles.
- Empty input → `PermissionSet.Empty`.
- Null input → throws `ArgumentNullException`.

## `ScopePermissionMap.For(string)`

**Signature**: `PermissionSet For(string scope)`

**Contract**:
- Returns the configured `PermissionSet` for `scope` (exact-match on scope string).
- Unmapped scope: returns `PermissionSet.Empty` and logs `Serilog.Log.Error("Unmapped scope: {Scope}", scope)` exactly once per scope per process.
- Null/empty input → throws `ArgumentException`.

## `ScopePermissionMap.For(IEnumerable<string>)`

**Signature**: `PermissionSet For(IEnumerable<string> scopes)`

**Contract**:
- Returns the union across all scopes.
- Empty input → `PermissionSet.Empty`.
- Null input → throws `ArgumentNullException`.
- Caller responsible for splitting the raw `scope` claim on space.

## `IUserRestrictionsProvider` (DI-scoped)

**Contract**:
- Lifetime: HTTP request (`AddScoped<IUserRestrictionsProvider, ...>()`).
- All methods memoise their result for the lifetime of the current request; the first call fetches from SQL Server read-models, subsequent calls return the cached collection.
- Never fetches from ElasticSearch. Never triggers event replay.
- Anonymous / permission-less users: methods return empty sets / `false`.
- Cancellation honoured via `CancellationToken`.

Key methods:
- `Task<IReadOnlySet<Guid>> GetOrganisationIdsAsync(CancellationToken)`
- `Task<IReadOnlySet<string>> GetOvoNumbersAsync(CancellationToken)`
- `Task<IReadOnlySet<Guid>> GetBodyIdsAsync(CancellationToken)`
- `Task<IReadOnlySet<Guid>> GetRegulationIdsAsync(CancellationToken)`
- `Task<bool> IsInScopeForOrganisationAsync(Guid, CancellationToken)`
- `Task<bool> IsInScopeForOrganisationAsync(string ovoNumber, CancellationToken)`
- `Task<bool> IsInScopeForBodyAsync(Guid, CancellationToken)`

## `OrganisationRegistryAuthorizeAttribute` (usage contract)

New parameter: `Permission[] RequiredPermissions`. Semantics: user must have at least one of the listed permissions (OR).

Old parameter (`Role[] Roles`): **removed** at cutover.

Example:
```csharp
[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.CanEditChildren })]
public class SomeController : Controller { ... }
```

## `ISecurityPolicy` (unchanged signature)

`AuthorizationResult Check(IUser user)` — implementations refactored to:
1. Check specific permission(s) via `HasPermission`.
2. Apply resource-level scope restrictions via `IUserRestrictionsProvider` (injected).

No breaking change to the `ISecurityPolicy` interface itself.
