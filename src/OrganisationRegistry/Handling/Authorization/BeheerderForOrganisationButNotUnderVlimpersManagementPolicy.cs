namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

public class BeheerderForOrganisationButNotUnderVlimpersManagementPolicy : ISecurityPolicy
{
    private readonly bool _isUnderVlimpersManagement;
    private readonly string _ovoNumber;

    public BeheerderForOrganisationButNotUnderVlimpersManagementPolicy(
        bool isUnderVlimpersManagement,
        string ovoNumber)
    {
        _isUnderVlimpersManagement = isUnderVlimpersManagement;
        _ovoNumber = ovoNumber;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInRole(Role.AlgemeenBeheerder))
            return AuthorizationResult.Success();

        if (!_isUnderVlimpersManagement &&
            user.IsDecentraalBeheerderFor(_ovoNumber))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    public override string ToString()
        => "Geen machtiging op organisatie";
}
