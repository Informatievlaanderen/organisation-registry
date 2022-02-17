namespace OrganisationRegistry.Handling.Authorization
{
    using Infrastructure.Authorization;
    using Organisation.Exceptions;

    public class BeheerderForOrganisationPolicy : ISecurityPolicy
    {
        private readonly bool _isUnderVlimpersManagement;
        private readonly string _ovoNumber;

        public BeheerderForOrganisationPolicy(
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

            if (!_isUnderVlimpersManagement &&
                user.IsOrganisatieBeheerderFor(_ovoNumber))
                return AuthorizationResult.Success();

            return _isUnderVlimpersManagement
                ? AuthorizationResult.Fail(new UserIsNotAuthorizedForVlimpersOrganisations())
                : AuthorizationResult.Fail(new InsufficientRights());
        }
    }
}
