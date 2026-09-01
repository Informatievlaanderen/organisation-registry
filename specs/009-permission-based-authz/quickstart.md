# Quickstart: Adding a New Permission

## When to add a permission

Add a new `Permission` enum value when introducing a capability that is:
- Checked at a controller/attribute boundary, or
- Checked inside an `ISecurityPolicy` as a general "is this user allowed to X?" question.

Do **not** add a permission for scope restrictions (which organisations/bodies/etc. a user may act on) — those are evaluated inside policies using `IUserRestrictionsProvider`, fetched JIT from SQL Server projections and memoised per request.

## Steps

### 1. Add the enum value

`src/OrganisationRegistry.Infrastructure/Authorization/Permission.cs`:

```csharp
public enum Permission
{
    // ...existing...
    CanManageWidgets,   // new — PascalCase, identical in code/docs/UI
}
```

### 2. Map roles → permission

`src/OrganisationRegistry.Infrastructure/Authorization/RolePermissionMap.cs`:

```csharp
Map[Role.AlgemeenBeheerder] = PermissionSet.Of(/* ...existing..., */ Permission.CanManageWidgets);
Map[Role.WidgetBeheerder]   = PermissionSet.Of(Permission.CanManageWidgets);
```

### 3. Map scopes → permission (only if reachable via Client Credentials)

`src/OrganisationRegistry.Infrastructure/Authorization/ScopePermissionMap.cs`:

```csharp
Map["dv_organisatieregister_widgetbeheerder"] = PermissionSet.Of(Permission.CanManageWidgets);
```

Skip if the capability is user-only (interactive login). Automated processes never carry a role — they enter via the scope path only.

### 4. Guard the controller

```csharp
[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.CanManageWidgets })]
public class WidgetController : OrganisationRegistryController { ... }
```

OR semantics: any listed permission satisfies.

### 5. Guard the domain (if needed)

Inside an `ISecurityPolicy`:

```csharp
public AuthorizationResult Check(IUser user)
{
    if (user.HasPermission(Permission.CanManageWidgets))
        return AuthorizationResult.Success();

    return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
}
```

For scope-restriction checks (e.g. "may edit *this* organisation"), inject `IUserRestrictionsProvider`:

```csharp
var restrictions = await _restrictions.GetForAsync(user, ct);
if (restrictions.Organisations.Contains(organisationId))
    return AuthorizationResult.Success();
```

`IUserRestrictionsProvider` is registered scoped; results are memoised for the duration of the HTTP request. Underlying reads hit SQL Server projections directly.

### 6. Test

- **Unit**: extend `RolePermissionMapTests` and `ScopePermissionMapTests` — assert new permission on expected roles/scopes.
- **Integration**: add one e2e test per relevant entry point (edit-api role claim, token-exchange role claim, bearer/CC scope claim) verifying the permission ends up in the resulting `User.Permissions`.
- **Policy tests**: mock `IUserRestrictionsProvider` for scope-restriction paths.

## Anti-patterns

- **Do not** reference `Role` or raw scope strings in controllers or policies. They live only in the three entry-point translation sites (`RolePermissionMap` / `ScopePermissionMap`).
- **Do not** add a permission for a one-off scope restriction. Restrictions come from `IUserRestrictionsProvider`, not `Permission`.
- **Do not** cache restrictions on `IUser` or in the security-user-object — always fetch JIT via the provider.
- **Do not** re-introduce `IsInAnyOf` or `Role[]` on `IUser`.
- **Do not** introduce lowercase/camelCase aliases for permission names. PascalCase in C#, docs, UI — identical string.
- **Do not** re-introduce the `AutomatedTask` role. Automated flows enter via CC scopes.
