namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing (global) reference data that has no
/// organisation context. Access is driven entirely by the presence of a
/// <see cref="Permission"/> grant, regardless of any restriction on that grant.
///
/// Because reference data (locations, capacities, function types, formal frameworks, ...)
/// is not tied to a single organisation, any holder of the permission — whether the grant
/// is unrestricted (e.g. AlgemeenBeheerder) or restricted to a scope (e.g. Decentraalbeheerder)
/// — is allowed to manage it.
/// </summary>
public class RequiresPermissionPolicy : ISecurityPolicy
{
    private readonly Permission _permission;

    public RequiresPermissionPolicy(Permission permission)
    {
        _permission = permission;
    }

    public AuthorizationResult Check(IUser user)
        => user.HasPermission(_permission)
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging om deze actie uit te voeren";
}
