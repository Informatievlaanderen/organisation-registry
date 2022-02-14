namespace OrganisationRegistry.Handling.Authorization
{
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
            if (user.IsInRole(Role.OrganisationRegistryBeheerder))
                return AuthorizationResult.Success();

            if (_isUnderVlimpersManagement &&
                user.IsAuthorizedForVlimpersOrganisations)
                return AuthorizationResult.Success();

            if (!_isUnderVlimpersManagement &&
                user.IsInRole(Role.OrganisatieBeheerder) &&
                user.Organisations.Contains(_ovoNumber))
                return AuthorizationResult.Success();

            return _isUnderVlimpersManagement
                ? AuthorizationResult.Fail(new UserIsNotAuthorizedForVlimpersOrganisations())
                : AuthorizationResult.Fail(new InsufficientRights());
        }
    }
}
