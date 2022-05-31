namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

public class AdminOnlyPolicy : ISecurityPolicy
{
    public AuthorizationResult Check(IUser user)
    {
        return user.IsInRole(Role.AlgemeenBeheerder) ?
            AuthorizationResult.Success() :
            AuthorizationResult.Fail(new InsufficientRights());
    }
}
