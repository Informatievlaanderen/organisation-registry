# Restrictions Architecture — Handler-Policy Migration

**Feature:** 009-permission-based-authz
**Status:** DONE — Keys policy implemented (commit c3b0d4af5)
**Scope:** MVP completes **`KeyPolicy`** as the reference implementation. Other
policies (Label, Capacity, FormalFramework, OrganisationClassificationType,
Regulation) follow the same pattern with per-domain context types.
**Related docs:**
- `ui-permission-matrix.md` — target-state per role/resource
- `security-architecture.md` — permission model, entry points, translation

## 1. Problem Statement

Handler-side policies currently mix three concerns:

1. **Role checks** (`user.IsInAnyOf(Role.VlimpersBeheerder)`) — must be replaced
   by permission checks per the feature's spec.
2. **Resource-id allowlist restrictions** — e.g. Vlimpers may only manage keys
   whose `KeyTypeId` is in `configuration.KeyIdsAllowedForVlimpers`.
3. **OVO-scope checks** — e.g. Decentraal may only act on his own organisation.

The permission model already handles concern (1) at controller level via
`RolePermissionMap` / `ScopePermissionMap`. Handler policies still need (2) and
(3). This document defines the API for (2). Concern (3) stays as-is: policies
call `ISecurityService` live for OVO scope, orthogonal to restrictions.

## 2. Design Goals

- **Data-driven per role.** Restrictions live in `RolePermissionMap`, next to
  the `Permission` they qualify. No code branch per role in policies.
- **Type-safe per resource domain.** Each restriction domain has its own
  context record (e.g. `KeyContext`), so policies can't accidentally
  pass the wrong shape.
- **Union across multi-role users.** If any of the user's entries for the same
  permission says YES, access is granted.
- **Fail-closed on missing mapping.** No entry for permission → deny.
- **Whitelist-only for MVP.** No `DenyListRestriction`; target-state matrix is
  pure allowlist (see `ui-permission-matrix.md`).
- **Backend code change acceptable** for new resource domains (semi-dynamic).
- **Permission-first call-site.** Role mapping reads
  `Permission.X.RestrictedTo(...)` — the permission stays visually first, the
  restriction hangs off it. No hidden coupling via separate factory names.
- **Non-generic restriction interface.** All restrictions implement `IRestriction`,
  and all contexts implement `IRestrictionContext`. Type safety within a grant
  is enforced via generics on `AllowListRestriction<TContext>` and
  `CompositeAndRestriction` composition.

## 3. Core Types

### 3.1 `IRestriction` and `AllowListRestriction<TContext>`

```csharp
namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

public interface IRestriction
{
    bool IsOkWith(IRestrictionContext context);
}

public sealed class AllowListRestriction<TContext> : IRestriction
    where TContext : IRestrictionContext
{
    private readonly ImmutableHashSet<Guid> _allowed;

    public AllowListRestriction(IEnumerable<Guid>? allowed)
    {
        _allowed = allowed is null
            ? ImmutableHashSet<Guid>.Empty
            : ImmutableHashSet.CreateRange(allowed);
    }

    public bool IsOkWith(IRestrictionContext context)
        => context is TContext typed && typed.RelevantIds.All(_allowed.Contains);
}
```

**Semantics:** the restriction says YES iff *every* id in `context.RelevantIds`
is in the allowlist. Empty relevant-id set yields vacuous truth ("nothing to
check").

**Type-safety:** the generic `TContext` tag documents which context this
allow-list expects (e.g., `AllowListRestriction<KeyContext>` for keys). At
runtime, it fails closed when handed a context that does not implement
`TContext`.

### 3.2 `CompositeAndRestriction`

Within a single grant, multiple restrictions combine as AND — the context must
satisfy *every* component restriction. This allows expressing compound guards
like "keytype must be in the allowed set AND organisation must be under Vlimpers
management" within one permission entry:

```csharp
public sealed class CompositeAndRestriction : IRestriction
{
    private readonly ImmutableHashSet<IRestriction> _restrictions;

    public CompositeAndRestriction(IEnumerable<IRestriction> restrictions)
    {
        _restrictions = restrictions is null
            ? ImmutableHashSet<IRestriction>.Empty
            : ImmutableHashSet.CreateRange(restrictions);
    }

    public bool IsOkWith(IRestrictionContext context)
        => _restrictions.All(r => r.IsOkWith(context));
}
```

**When to use AND-composition:** within a single grant (one permission for one
role), when you need both conditions to pass. **OR semantics** live at the
`PermissionSet` level: different roles contribute separate grants, and a
permission is satisfied if *any* grant applies (see §4).

### 3.3 `IRestrictionContext` — Non-Generic Marker

The restriction algebra is non-generic at the interface level for simplicity.
All contexts implement the base marker:

```csharp
public interface IRestrictionContext
{
    /// <summary>
    /// The ids that must all be allowed for the operation to be permitted.
    /// An empty enumerable means "nothing to check" and yields vacuous truth.
    /// </summary>
    IEnumerable<Guid> RelevantIds { get; }
}

/// <summary>
/// Capability a context exposes when it can report whether the organisation
/// the operation targets is under Vlimpers management.
/// </summary>
public interface IVlimpersManagedContext : IRestrictionContext
{
    bool IsUnderVlimpersManagement { get; }
}
```

Per-domain contexts are concrete records implementing `IRestrictionContext` or
`IVlimpersManagedContext`:

```csharp
// MVP key context
public sealed record KeyContext(bool IsUnderVlimpersManagement, IReadOnlyCollection<Guid> KeyTypeIds)
    : IVlimpersManagedContext
{
    public KeyContext(bool isUnderVlimpersManagement, Guid keyTypeId)
        : this(isUnderVlimpersManagement, new[] { keyTypeId }) { }

    public IEnumerable<Guid> RelevantIds => KeyTypeIds;
}
```

**Design note:** contexts contain *only* resource-id data and capability flags
(like `IsUnderVlimpersManagement`). OVO/org-scope is not part of the context;
policies handle that via `ISecurityService` live lookup.

## 4. Storage: `PermissionEntry` inside `PermissionSet`

Restrictions live inside `PermissionSet` — no parallel channel:

```csharp
public sealed record PermissionEntry(
    Permission Permission,
    IRestriction? Restriction)    // null = unrestricted
```

**Unrestricted case:** when `Restriction` is `null`, the grant is unrestricted.
This is the natural way to express "permission held without any resource
constraint".

**Absorbing logic:** `PermissionSet.IsSatisfiedFor(permission, context)` returns
true if *any* entry for that permission applies. An unrestricted entry (`Restriction`
is `null`) always applies, so it naturally absorbs restricted entries for the
same permission. Example:

```csharp
// Scenario: AlgemeenBeheerder has CanManageKeys unrestricted,
// VlimpersBeheerder has CanManageKeys restricted to their keytype set.
// Union produces both entries. IsSatisfiedFor checks:
// - Is there an entry for CanManageKeys whose restriction is satisfied?
// - If any entry has null Restriction (unrestricted) → YES
// - Otherwise, check each restricted entry's IsOkWith(context)

set.IsSatisfiedFor(Permission.CanManageKeys, context)
    => entries.Any(e =>
        e.Permission == permission &&
        (e.Restriction is null || e.Restriction.IsOkWith(context)));
```

This means multi-role users automatically get the union: each role contributes
its own entries, and if any entry says YES, access is granted.

## 5. `IUser` Surface

The core authorization decision is simple and non-generic:

```csharp
bool HasPermission(Permission permission);

bool IsSatisfiedFor(Permission permission, IRestrictionContext context);
```

**Semantics:**

- `HasPermission(permission)` — YES if the user has any entry for the
  permission, restricted or not. Used by controllers for early gates.
- `IsSatisfiedFor(permission, context)` — core enforcement. YES when there is
  at least one grant for the permission whose restriction (if any) is satisfied
  by the context. Returns false if no entry for the permission exists (fail-closed).

**Example:**

```csharp
public interface IUser
{
    PermissionSet Permissions { get; }
    bool HasPermission(Permission permission);
    bool IsSatisfiedFor(Permission permission, IRestrictionContext context);
}

public class User : IUser
{
    public PermissionSet Permissions { get; }

    public bool HasPermission(Permission permission)
        => Permissions.Contains(permission);

    public bool IsSatisfiedFor(Permission permission, IRestrictionContext context)
        => Permissions.IsSatisfiedFor(permission, context);
}
```

The `PermissionSet` itself handles all the hard work:

```csharp
public bool IsSatisfiedFor(Permission permission, IRestrictionContext context)
    => _entries.Any(e =>
        e.Permission == permission &&
        (e.Restriction is null || e.Restriction.IsOkWith(context)));
```

## 6. Reference: `KeyPolicy` After Migration

```csharp
public class KeyPolicy : ISecurityPolicy
{
    private readonly bool _isUnderVlimpersManagement;
    private readonly Guid[] _keyTypeIds;

    public KeyPolicy(bool isUnderVlimpersManagement, params Guid[] keyTypeIds)
    {
        _isUnderVlimpersManagement = isUnderVlimpersManagement;
        _keyTypeIds = keyTypeIds;
    }

    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(
            Permission.CanManageKeys,
            new KeyContext(_isUnderVlimpersManagement, _keyTypeIds))
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString() => "Geen machtiging op sleutel";
}
```

**Key observations:**

1. The policy is permission-first: it checks `Permission.CanManageKeys` directly.
2. The restriction is evaluated via `user.IsSatisfiedFor(permission, context)`.
3. The context bundles both the organisation's Vlimpers-management status and
   the keytype ids, so the restriction can enforce both checks in one pass.
4. No `IOrganisationRegistryConfiguration` parameter — the allowlist is baked
   into the permission mapping at startup (see §7.3).

## 7. Role Mapping: Permission-First Syntax

Restrictions attach to a `Permission` via a single extension method. The
permission stays visually first; the restriction hangs off it. There is no
separate "restriction factory" name to remember — the coupling is explicit at
every call site.

### 7.1 `RestrictedTo` Extension

```csharp
public static class PermissionExtensions
{
    /// <summary>
    /// Pairs a Permission with a restriction that the operation's
    /// IRestrictionContext must satisfy for the grant to apply.
    /// </summary>
    public static PermissionEntry RestrictedTo(
        this Permission permission,
        IRestriction restriction)
        => new(permission, restriction);
}
```

**Usage:** `Permission.CanManageKeys.RestrictedTo(someRestriction)`

### 7.2 Per-Domain Helpers

To keep the allowlist source obvious, each domain exposes a small helper class
that returns `IRestriction`. This is a thin convenience layer over
`AllowListRestriction<TContext>`:

```csharp
public static class KeyRestrictions
{
    /// <summary>
    /// The keytype ids the caller may touch must all be in keyTypeIds.
    /// </summary>
    public static IRestriction AllowList(IEnumerable<Guid> keyTypeIds)
        => new AllowListRestriction<KeyContext>(keyTypeIds);

    /// <summary>
    /// Vlimpers grant for keys: the organisation must be under Vlimpers management
    /// AND every touched keytype must be in the Vlimpers-allowed set. Both
    /// conditions live in a single grant (AND); other roles express their own key
    /// access as separate grants (OR).
    /// </summary>
    public static IRestriction VlimpersManaged(IEnumerable<Guid> keyTypeIds)
        => new CompositeAndRestriction(
            RequireUnderVlimpersManagementRestriction.Instance,
            new AllowListRestriction<KeyContext>(keyTypeIds));
}
```

Analogous helpers per domain (e.g., `LabelRestrictions.AllowList(...)`,
`CapacityRestrictions.AllowList(...)`, etc.).

### 7.3 Call Site in `RolePermissionMap`

`RolePermissionMap` has two static methods:

```csharp
/// Config-less: just unions the static base permissions per role.
public static PermissionSet For(IEnumerable<Role>? roles, ILogger? logger = null)
{
    var union = PermissionSet.Empty;
    foreach (var role in roles)
        union = union.Union(For(role, logger));
    return union;
}

/// Config-aware: adds data-driven restricted grants.
public static PermissionSet For(
    IEnumerable<Role>? roles,
    IOrganisationRegistryConfiguration configuration,
    ILogger? logger = null)
{
    var roleList = roles as IReadOnlyCollection<Role> ?? roles.ToList();
    var union = For(roleList, logger);  // Start with static base

    // Layer in config-dependent restricted grants per role
    foreach (var role in roleList)
        union = union.Union(RestrictedGrantsFor(role, configuration));

    return union;
}

private static PermissionSet RestrictedGrantsFor(
    Role role,
    IOrganisationRegistryConfiguration configuration)
    => role switch
    {
        Role.VlimpersBeheerder => PermissionSet.Of(
            Permission.CanManageKeys.RestrictedTo(
                KeyRestrictions.VlimpersManaged(
                    configuration.Authorization.KeyIdsAllowedForVlimpers))),
        _ => PermissionSet.Empty,
    };
```

**Call site in `User.cs`:**

```csharp
public User(
    ...,
    PermissionSet? permissions = null)
{
    ...
    Permissions = permissions ?? RolePermissionMap.For(roles);
}
```

When config is available (e.g., in `SecurityService`):

```csharp
return new User(
    ...,
    RolePermissionMap.For(securityInformation.Roles, _configuration));
```

The `Permission → RestrictedTo → helper` chain reads left-to-right as
"CanManageKeys, restricted to the VlimpersManaged keytypes". The
`PermissionSet.Of(...)` builder accepts both `Permission` and `PermissionEntry`
(via `implicit operator PermissionEntry(Permission)`), so unrestricted
permissions stay one-word.

## 8. Decisions Recap (from summary)

| Topic | Decision |
|---|---|
| `Permission` source-of-truth | Enum stays (option b) |
| Restriction storage | Inside `PermissionSet` via `PermissionEntry` — no parallel channel |
| Unrestricted representation | `Restriction` field is `null` (not a wrapper class) |
| Restriction logic within a grant | `CompositeAndRestriction` — AND composition |
| Multi-role union | OR across grants: each role contributes entries to `PermissionSet`; any entry that says YES grants access |
| Context shape | Named record per domain, resource-ids + capability flags (e.g., `IsUnderVlimpersManagement`), no OVO |
| OVO-scope | Stays in policy code via `ISecurityService` live |
| MVP restriction type | `AllowListRestriction` only (no DenyList) |
| `KeyRestrictions.VlimpersManaged` | `CompositeAndRestriction` of `RequireUnderVlimpersManagementRestriction` AND `AllowListRestriction<KeyContext>` |
| DecentraalBeheerder + Keys | **No** `CanManageKeys` (target-state per matrix) |

## 9. Retirement Checklist

Once a policy migrates to the restriction model, confirm these cleanups:

- `SecurityService.CanUseLabelType` remains (L233-243 in `SecurityService.cs`) —
  this is a special-case gate for Vlimpers-only label types, **not** replaced by
  the restriction model. Left in place for now (deferred cleanup: see AGENTS.md
  note "TODO: see how we can make SecurityService use IUser everywhere").
- `KeyPolicy` no longer takes `IOrganisationRegistryConfiguration` parameter
  (moved to `RolePermissionMap` construction).
- `SecurityService.CanUseKeyType` — removed (no longer needed).

Analogous cleanups follow for each subsequent policy migration.

## 10. Open Ambiguities

These need product confirmation before their respective policy migrates
(see `ui-permission-matrix.md` § Interpretatie per sterretje-cel):

- **DB Sleutels R (vs huidig CRUD op subset).** Confirmed: DecentraalBeheerder
  loses `CanManageKeys`. Migration includes this reduction.
- **DB Benamingen `*`.** Which label types? Currently "everything except
  `LabelIdsAllowedForVlimpers`". Product must decide before `LabelPolicy`
  migrates.
- **VB Hoedanigheden `R*`.** Restricted read — likely a new capability, not a
  policy migration. Deferred.
- **DB Hoedanigheden / DB Toepassingsgebieden `*`.** Presumably OVO-scope only
  (no id restriction) — confirm before `CapacityPolicy` / `FormalFrameworkPolicy`
  migrates.
- **RDB Classificaties `*`.** `OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder`
  whitelist confirmed; Cjm via CC scope open.

## 11. Rollout Status

| Resource | Policy | Handler | Projection | Status |
|---|---|---|---|---|
| **Organisation Key** | `KeyPolicy` | `AddOrganisationKeyCommandHandler`, `UpdateOrganisationKeyCommandHandler` | `OrganisationKeyController`, `KeyTypeController` | ✅ DONE (commit c3b0d4af5) |
| Organisation Label | `LabelPolicy` | `AddOrganisationLabelCommandHandler`, `UpdateOrganisationLabelCommandHandler` | — | 🔄 Deferred (depends on DB Benamingen `*` clarification) |
| Organisation Capacity | `CapacityPolicy` | `AddOrganisationCapacityCommandHandler`, `UpdateOrganisationCapacityCommandHandler` | — | 🔄 Deferred (depends on DB Hoedanigheden `*` clarification) |
| Organisation FormalFramework | `FormalFrameworkPolicy` | `AddOrganisationFormalFrameworkCommandHandler`, `UpdateOrganisationFormalFrameworkCommandHandler` | — | 🔄 Deferred (depends on DB Toepassingsgebieden `*` clarification) |
| Organisation OrganisationClassificationType | `OrganisationClassificationTypePolicy` | `AddOrganisationOrganisationClassificationCommandHandler`, `UpdateOrganisationOrganisationClassificationCommandHandler` | — | 🔄 Deferred (depends on RDB Cjm scope clarification) |
| Organisation Regulation | `RegulationPolicy` | — | — | 🔄 Deferred (pure permission check, no restriction needed) |

**Keys implementation:** `AddOrganisationKeyCommandHandler` and `UpdateOrganisationKeyCommandHandler`
send commands that trigger `KeyPolicy`. The controller (`OrganisationKeyController`) and
key-type controller (`KeyTypeController`) are fully wired.

## 12. FluentAssertions Gotcha

`PermissionSet : IEnumerable<PermissionEntry>` will trigger FluentAssertions'
collection overload on `.Should().Be(other)`. Use `((object)set).Should().Be(other)`
when comparing whole sets for equality in tests.

## 13. Restriction Types & Special Cases

### 13.1 `RequireUnderVlimpersManagementRestriction`

A stateless singleton that passes only when the context carries the
`IVlimpersManagedContext` capability and reports the organisation is under
Vlimpers management:

```csharp
public sealed class RequireUnderVlimpersManagementRestriction : IRestriction
{
    public static readonly RequireUnderVlimpersManagementRestriction Instance = new();

    private RequireUnderVlimpersManagementRestriction() { }

    public bool IsOkWith(IRestrictionContext context)
        => context is IVlimpersManagedContext vlimpers && vlimpers.IsUnderVlimpersManagement;

    public override string ToString() => "RequireUnderVlimpersManagement";
}
```

**Usage:** Combined with `AllowListRestriction<KeyContext>` via
`CompositeAndRestriction` to form `KeyRestrictions.VlimpersManaged(keyTypeIds)`.

### 13.2 Core Files in `Restrictions/`

| File | Contents |
|---|---|
| `IRestrictionContext.cs` | `IRestrictionContext` and `IVlimpersManagedContext` marker interfaces |
| `IRestriction.cs` | `IRestriction` interface with `IsOkWith(context)` |
| `AllowListRestriction.cs` | Generic `AllowListRestriction<TContext>` with `ImmutableHashSet` internals |
| `CompositeAndRestriction.cs` | AND-composition of multiple restrictions |
| `RequireUnderVlimpersManagementRestriction.cs` | Singleton gate for Vlimpers-managed organisations |
| `KeyContext.cs` | `KeyContext` record, carries keytype ids and `IsUnderVlimpersManagement` flag |
| `KeyRestrictions.cs` | `KeyRestrictions.AllowList(ids)` and `KeyRestrictions.VlimpersManaged(ids)` factories |

Files touched (not new):

| File | Change |
|---|---|
| `Authorization/IUser.cs` | Already has `IsSatisfiedFor(permission, context)` |
| `Authorization/PermissionSet.cs` | `IEnumerable<PermissionEntry>`, `Of(...)` accepts both `Permission` and `PermissionEntry`; core decision is `IsSatisfiedFor` |
| `Authorization/PermissionEntry.cs` | `PermissionEntry(Permission, IRestriction?)` record with `implicit operator` |
| `Authorization/PermissionExtensions.cs` | `RestrictedTo(this Permission, IRestriction)` extension |
| `Authorization/RolePermissionMap.cs` | Two overloads: `For(roles)` and `For(roles, config, logger)`; config-less version in `User.cs` ctor |
| `Handling/Authorization/KeyPolicy.cs` | Takes `isUnderVlimpersManagement` flag and `keyTypeIds`, calls `user.IsSatisfiedFor` with `KeyContext` |
| `Api/Security/SecurityService.cs` | Calls `RolePermissionMap.For(roles, _configuration)` in `GetRequiredUser` and `GetUser`; `CanUseLabelType` remains (special-case gate) |
