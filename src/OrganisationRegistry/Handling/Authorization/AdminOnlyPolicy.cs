namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

public class AdminOnlyPolicy : ISecurityPolicy
{
    public AuthorizationResult Check(IUser user)
        => user.IsInRole(Role.AlgemeenBeheerder) ?
            AuthorizationResult.Success() :
            AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging om deze actie uit te voeren";
}
