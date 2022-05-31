namespace OrganisationRegistry.Handling.Authorization;

using System;
using System.Linq;
using Infrastructure.Authorization;
using Organisation.Exceptions;

public class RequiresRolesPolicy : ISecurityPolicy
{
    private readonly Role[] _roles;

    public RequiresRolesPolicy(params Role[] roles)
    {
        if (roles == null)
            throw new ArgumentNullException(nameof(roles));

        if (roles.Length == 0)
            throw new ArgumentException("At least one role needs to be provided", nameof(roles));

        _roles = roles;
    }
    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInRole(Role.AlgemeenBeheerder))
            return AuthorizationResult.Success();

        if (_roles.Any(user.IsInRole))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(new InsufficientRights());
    }
}
