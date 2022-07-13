namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

public class RegulationPolicy : ISecurityPolicy
{
    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInAnyOf(Role.AlgemeenBeheerder))
            return AuthorizationResult.Success();

        if (user.IsInAnyOf(Role.RegelgevingBeheerder))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    public override string ToString()
        => "Geen machtiging op regelgeving";
}
