namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

public class VlimpersPolicy : ISecurityPolicy
{
    private readonly bool _isUnderVlimpersManagement;
    private readonly string _ovoNumber;

    public VlimpersPolicy(
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

        if (_isUnderVlimpersManagement &&
            user.IsAuthorizedForVlimpersOrganisations)
            return AuthorizationResult.Success();

        if (!_isUnderVlimpersManagement &&
            user.IsDecentraalBeheerderFor(_ovoNumber))
            return AuthorizationResult.Success();

        return _isUnderVlimpersManagement
            ? AuthorizationResult.Fail(new UserIsNotAuthorizedForVlimpersOrganisations())
            : AuthorizationResult.Fail(new InsufficientRights<VlimpersPolicy>(this));
    }

    public override string ToString()
        => "Geen machtiging op deze organisatie (Vlimpers gerelateerd).";
}
