# Restrictions Architecture — Handler-Policy Migration

**Feature:** 009-permission-based-authz
**Status:** Proposal — pending user go-ahead
**Scope:** MVP focuses on **`KeyPolicy`** as the reference implementation. Other
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
  context record (e.g. `KeyRestrictionContext`), so policies can't accidentally
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
- **Domain identity lives on the context type.** Each `TContext` implements
  `IRestrictionContext` with a `static abstract string Domain`, so
  `IUser.GetRestriction<TContext>()` needs no string argument and the domain
  key cannot drift from the context shape.

## 3. Core Types

### 3.1 `IRestriction<TContext>` and `AllowListRestriction<TContext>`

```csharp
namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

public interface IRestriction<in TContext>
{
    bool IsOkWith(TContext context);
}

public sealed class AllowListRestriction<TContext> : IRestriction<TContext>
{
    private readonly IReadOnlySet<Guid> _allowedIds;
    private readonly Func<TContext, IEnumerable<Guid>> _extractIds;

    public AllowListRestriction(
        IEnumerable<Guid> allowedIds,
        Func<TContext, IEnumerable<Guid>> extractIds)
    {
        _allowedIds = new HashSet<Guid>(allowedIds);
        _extractIds = extractIds;
    }

    public bool IsOkWith(TContext context)
        => _extractIds(context).All(_allowedIds.Contains);
}
```

**Semantics:** the restriction says YES iff *every* id extracted from the
context is in the allowlist. Empty context (no ids requested) → YES.

### 3.2 `CompositeOrRestriction<TContext>`

For multi-role users we union permission entries. When two entries carry
different restrictions for the same permission, we combine them:

```csharp
public sealed class CompositeOrRestriction<TContext> : IRestriction<TContext>
{
    private readonly IReadOnlyList<IRestriction<TContext>> _inner;
    public CompositeOrRestriction(IEnumerable<IRestriction<TContext>> inner)
        => _inner = inner.ToList();

    public bool IsOkWith(TContext context)
        => _inner.Any(r => r.IsOkWith(context));
}
```

The unrestricted case (permission held without restriction) is represented by
`UnrestrictedRestriction<TContext>` which always returns `true`. Union of any
restriction with `Unrestricted` collapses to `Unrestricted`.

### 3.3 `IRestrictionContext` and Per-Domain Context Records

Each resource domain defines its own context record, which carries its own
domain identifier as a static-abstract member:

```csharp
public interface IRestrictionContext<TSelf>
    where TSelf : IRestrictionContext<TSelf>
{
    static abstract string Domain { get; }
}
```

MVP context:

```csharp
public sealed record KeyRestrictionContext(IReadOnlyList<Guid> KeyTypeIds)
    : IRestrictionContext<KeyRestrictionContext>
{
    public static string Domain => "OrganisationKeys";
}
```

Future domains sketched (not implemented yet):

```csharp
public sealed record LabelRestrictionContext(
    IReadOnlyList<Guid> LabelTypeIds,
    bool IsUnderVlimpersManagement)
    : IRestrictionContext<LabelRestrictionContext>
{
    public static string Domain => "OrganisationLabels";
}

public sealed record CapacityRestrictionContext(IReadOnlyList<Guid> CapacityIds)
    : IRestrictionContext<CapacityRestrictionContext>
{
    public static string Domain => "OrganisationCapacities";
}

public sealed record FormalFrameworkRestrictionContext(IReadOnlyList<Guid> FormalFrameworkIds)
    : IRestrictionContext<FormalFrameworkRestrictionContext>
{
    public static string Domain => "OrganisationFormalFrameworks";
}

public sealed record OrganisationClassificationTypeRestrictionContext(
    IReadOnlyList<Guid> OrganisationClassificationTypeIds)
    : IRestrictionContext<OrganisationClassificationTypeRestrictionContext>
{
    public static string Domain => "OrganisationClassifications";
}
```

**Design note:** contexts contain *only* resource-id data. OVO/org-scope is not
part of the context; policies handle that via `ISecurityService` live lookup.

### 3.4 Domain Keys

Domain keys are **not** centralised in a separate constants class. The single
source of truth is `TContext.Domain` on each context record. `PermissionEntry`
stores the string (§4) so union/lookup across the heterogeneous
`PermissionSet` stays simple, but call sites never type a raw string.

## 4. Storage: `PermissionEntry` inside `PermissionSet`

Restrictions live inside `PermissionSet` — no parallel channel:

```csharp
public readonly record struct PermissionEntry(
    Permission Permission,
    string? RestrictionDomain,   // null = unrestricted
    object? Restriction);        // IRestriction<TContext> boxed; null if unrestricted
```

`PermissionSet` becomes `IReadOnlyCollection<PermissionEntry>`. Union across
roles/scopes preserves entries; consumers (`HasPermission`, `GetRestriction`)
walk them.

**Why `object?` for `Restriction`:** the set is heterogeneous across domains.
Type safety is recovered at the `GetRestriction<TContext>` call site (see §5).

## 5. `IUser` Surface

Three new members on `IUser`, all generic — no string domain arguments at call
sites:

```csharp
bool HasPermission(Permission permission);

bool IsRestrictedTo<TContext>()
    where TContext : IRestrictionContext<TContext>;

IRestriction<TContext> GetRestriction<TContext>()
    where TContext : IRestrictionContext<TContext>;
```

Internally these read `TContext.Domain` and match against
`PermissionEntry.RestrictionDomain`.

**Semantics:**

- `HasPermission(p)` — YES if the user has any entry for `p`, restricted or not.
- `IsRestrictedTo<TContext>()` — YES if the user has at least one entry whose
  `RestrictionDomain == TContext.Domain` **and no unrestricted entry for the
  same permission** in that domain. Callers use this to decide "do I need to
  walk the restriction path?".
- `GetRestriction<TContext>()` — returns the effective restriction for
  `TContext.Domain`. If multiple entries carry restrictions for the same
  domain, they are combined into `CompositeOrRestriction<TContext>`. If the
  user has no entry for the domain, returns a `DenyAllRestriction<TContext>`
  (always `false`). Never returns null (avoids null checks in policies).

**Fail-closed:** missing domain → `DenyAll`, not `Unrestricted`.

## 6. Reference: `KeyPolicy` After Migration

```csharp
public class KeyPolicy : ISecurityPolicy
{
    private readonly Guid[] _keyTypeIds;
    private readonly string _ovoNumber;

    public KeyPolicy(string ovoNumber, params Guid[] keyTypeIds)
    {
        _ovoNumber = ovoNumber;
        _keyTypeIds = keyTypeIds;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (!user.HasPermission(Permission.CanManageKeys))
            return Fail();

        if (!user.IsRestrictedTo<KeyRestrictionContext>())
            return AuthorizationResult.Success();

        var restriction = user.GetRestriction<KeyRestrictionContext>();

        if (restriction.IsOkWith(new KeyRestrictionContext(_keyTypeIds)))
            return AuthorizationResult.Success();

        return Fail();
    }

    private AuthorizationResult Fail()
        => AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString() => "Geen machtiging op sleutel";
}
```

Note: `IOrganisationRegistryConfiguration` drops out of `KeyPolicy` entirely.
The allowlist moves to `RolePermissionMap` construction time.

## 7. Role Mapping: Permission-First Syntax

Restrictions attach to a `Permission` via a single extension method. The
permission stays visually first; the restriction hangs off it. There is no
separate "restriction factory" name to remember — the coupling is explicit at
every call site.

### 7.1 `RestrictedTo` Extension

```csharp
public static class PermissionRestrictionExtensions
{
    public static PermissionEntry RestrictedTo<TContext>(
        this Permission permission,
        IRestriction<TContext> restriction)
        where TContext : IRestrictionContext<TContext>
        => new(permission, TContext.Domain, restriction);
}
```

The domain string is read from `TContext.Domain` — call sites never type it.

### 7.2 Per-Domain Helpers

To keep the allowlist source obvious, each domain exposes a small helper class
that returns `IRestriction<TContext>`. This is a thin convenience layer over
`AllowListRestriction<TContext>` — no factories for `PermissionEntry`, only for
the restriction itself:

```csharp
public static class KeyRestrictions
{
    public static IRestriction<KeyRestrictionContext> AllowList(
        IEnumerable<Guid> allowedKeyTypeIds)
        => new AllowListRestriction<KeyRestrictionContext>(
            allowedKeyTypeIds,
            ctx => ctx.KeyTypeIds);
}
```

Analogous helpers per domain (`LabelRestrictions.AllowList(...)`,
`CapacityRestrictions.AllowList(...)`, etc.).

### 7.3 Call Site in `RolePermissionMap`

```csharp
[Role.VlimpersBeheerder] = PermissionSet.Of(
    Permission.CanEditVlimpers,
    Permission.CanEditChildren,
    Permission.CanManageKeys.RestrictedTo(
        KeyRestrictions.AllowList(configuration.Authorization.KeyIdsAllowedForVlimpers)),
    Permission.CanEditOrganisationLabels),
```

The `Permission → RestrictedTo → helper` chain reads left-to-right as
"CanManageKeys, restricted to the Vlimpers key allowlist". The
`PermissionSet.Of(...)` builder accepts both `Permission` and
`PermissionEntry` (via `implicit operator PermissionEntry(Permission)`), so
unrestricted permissions stay one-word.

## 8. Decisions Recap (from summary)

| Topic | Decision |
|---|---|
| `Permission` source-of-truth | Enum stays (option b) |
| Restriction storage | Inside `PermissionSet` via `PermissionEntry` — no parallel channel |
| Domain identifier | `static abstract string Domain` on `IRestrictionContext<TSelf>` per context record |
| Context shape | Named record per domain, resource-ids only, no OVO |
| OVO-scope | Stays in policy code via `ISecurityService` live |
| MVP restriction type | `AllowListRestriction` only (no DenyList) |
| Multi-role union | `CompositeOrRestriction` (ANY entry may say YES) |
| Missing domain | `DenyAllRestriction` (fail-closed) |
| DecentraalBeheerder + Keys | **No** `CanManageKeys` (target-state per matrix) |
| `LabelPolicy` `_isUnderVlimpersManagement` | Field on `LabelRestrictionContext` |

## 9. Retirement List

Once `KeyPolicy` migrates, the following can be removed or deprecated:

- `SecurityService.CanUseKeyType` (L231-245 in `SecurityService.cs`)
- `KeyPolicy` constructor param `IOrganisationRegistryConfiguration`
- Dead code L37+ in `KeyPolicy.cs`

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

## 11. Migration Order

Per-policy, each in its own commit:

1. **Infrastructure** — types from §§3-5 and §7 (`IRestriction<TContext>`,
   `IRestrictionContext<TSelf>`, `PermissionEntry`, per-domain
   `*RestrictionContext` records, `AllowListRestriction<TContext>`,
   `CompositeOrRestriction<TContext>`, `DenyAllRestriction<TContext>`,
   `UnrestrictedRestriction<TContext>`, `PermissionRestrictionExtensions.RestrictedTo`,
   per-domain helper (`KeyRestrictions`), `PermissionSet` refactor,
   `IUser` generic members).
2. **KeyPolicy** — reference migration. Drops config param. Adds Vlimpers
   entry in `RolePermissionMap` via
   `Permission.CanManageKeys.RestrictedTo(KeyRestrictions.AllowList(...))`.
   Removes dead code + `SecurityService.CanUseKeyType`.
3. **LabelPolicy** — with `IsUnderVlimpersManagement` on context. Requires
   DB-labels ambiguity resolved.
4. **CapacityPolicy** — needs DB `*` interpretation.
5. **FormalFrameworkPolicy** — needs DB `*` interpretation.
6. **OrganisationClassificationTypePolicy** — needs RDB Cjm decision.
7. **RegulationPolicy** — pure permission check (no restriction), simplest.

Each step includes: types → `RolePermissionMap` entry → policy rewrite → tests →
retire dead code / `SecurityService.CanUse*`.

## 12. FluentAssertions Gotcha

`PermissionSet : IEnumerable<Permission>` (existing) or the new
`IEnumerable<PermissionEntry>` shape will trigger FluentAssertions' collection
overload on `.Should().Be(other)`. Use `((object)set).Should().Be(other)` when
comparing whole sets for equality in tests.

## 13. File Plan (Infrastructure Step)

All new files live under
`src/OrganisationRegistry.Infrastructure/Authorization/Restrictions/`.

| File | Contents |
|---|---|
| `IRestrictionContext.cs` | `IRestrictionContext<TSelf>` interface (static-abstract `Domain`) |
| `IRestriction.cs` | `IRestriction<in TContext>` interface (`IsOkWith`) |
| `AllowListRestriction.cs` | `AllowListRestriction<TContext>` generic sealed class |
| `CompositeOrRestriction.cs` | `CompositeOrRestriction<TContext>` union combinator |
| `DenyAllRestriction.cs` | `DenyAllRestriction<TContext>` (returned when domain missing) |
| `UnrestrictedRestriction.cs` | `UnrestrictedRestriction<TContext>` (permission held without restriction) |
| `PermissionEntry.cs` | `PermissionEntry(Permission, string?, object?)` readonly record struct + `implicit operator PermissionEntry(Permission)` |
| `PermissionRestrictionExtensions.cs` | `RestrictedTo<TContext>(this Permission, IRestriction<TContext>)` |
| `Contexts/KeyRestrictionContext.cs` | MVP context record, implements `IRestrictionContext<KeyRestrictionContext>` |
| `Contexts/KeyRestrictions.cs` | `KeyRestrictions.AllowList(IEnumerable<Guid>)` helper |

Files touched (not new):

| File | Change |
|---|---|
| `Authorization/IUser.cs` | Add `HasPermission`, `IsRestrictedTo<TContext>`, `GetRestriction<TContext>` |
| `Authorization/PermissionSet.cs` | Backing store becomes `IReadOnlyCollection<PermissionEntry>`; `Of(...)` accepts both `Permission` and `PermissionEntry` |
| `Authorization/RolePermissionMap.cs` | Rewrite Vlimpers `Restricted(...)` scaffold (L127-153) to `Permission.CanManageKeys.RestrictedTo(KeyRestrictions.AllowList(configuration.Authorization.KeyIdsAllowedForVlimpers))` |
| `Handling/Authorization/KeyPolicy.cs` (core project) | Rewrite per §6; drop `IOrganisationRegistryConfiguration` param; delete dead code L37+ |
| `Api/Infrastructure/Security/SecurityService.cs` | Delete `CanUseKeyType` (L231-245) |

Future domains reuse the pattern — each adds one `*RestrictionContext` +
one `*Restrictions` helper file, plus its own policy migration.
