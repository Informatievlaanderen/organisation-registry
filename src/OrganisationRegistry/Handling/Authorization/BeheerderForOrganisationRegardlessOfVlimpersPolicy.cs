namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

public class BeheerderForOrganisationRegardlessOfVlimpersPolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;

    public BeheerderForOrganisationRegardlessOfVlimpersPolicy(string ovoNumber)
    {
        _ovoNumber = ovoNumber;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInRole(Role.AlgemeenBeheerder))
            return AuthorizationResult.Success();

        if (user.IsDecentraalBeheerderFor(_ovoNumber))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    public override string ToString()
        => "Geen machtiging op organisatie";
}
