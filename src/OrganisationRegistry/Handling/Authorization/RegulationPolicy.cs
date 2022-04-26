namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

public class RegulationPolicy : ISecurityPolicy
{
    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInRole(Role.AlgemeenBeheerder))
            return AuthorizationResult.Success();

        if (user.IsInRole(Role.RegelgevingBeheerder))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(new InsufficientRights());
    }
}
