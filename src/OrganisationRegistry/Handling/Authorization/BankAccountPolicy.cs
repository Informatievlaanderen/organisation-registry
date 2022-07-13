namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

public class BankAccountPolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;

    public BankAccountPolicy(string ovoNumber)
    {
        _ovoNumber = ovoNumber;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInAnyOf(Role.AlgemeenBeheerder, Role.CjmBeheerder))
            return AuthorizationResult.Success();

        if (user.IsDecentraalBeheerderFor(_ovoNumber))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    public override string ToString()
        => "Geen machtiging op organisatie";
}
