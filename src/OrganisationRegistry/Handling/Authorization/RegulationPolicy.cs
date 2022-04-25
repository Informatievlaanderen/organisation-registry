namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

public class RegulationPolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;

    public RegulationPolicy(string ovoNumber)
    {
        _ovoNumber = ovoNumber;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInRole(Role.AlgemeenBeheerder))
            return AuthorizationResult.Success();

        if (user.IsInRole(Role.RegelgevingBeheerder))
            return AuthorizationResult.Success();

        if (user.IsOrganisatieBeheerderFor(_ovoNumber))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(new InsufficientRights());
    }
}
