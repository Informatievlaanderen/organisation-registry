namespace OrganisationRegistry.Handling.Authorization
{
    using Infrastructure.Authorization;
    using Organisation.Exceptions;

    public class BeheerderForOrganisationPolicy : ISecurityPolicy
    {
        private readonly bool _allowOrganisationToBeUnderVlimpersManagement;
        private readonly bool _isUnderVlimpersManagement;
        private readonly string _ovoNumber;

        public BeheerderForOrganisationPolicy(
            bool isUnderVlimpersManagement,
            string ovoNumber,
            bool allowOrganisationToBeUnderVlimpersManagement)
        {
            _isUnderVlimpersManagement = isUnderVlimpersManagement;
            _ovoNumber = ovoNumber;
            _allowOrganisationToBeUnderVlimpersManagement = allowOrganisationToBeUnderVlimpersManagement;
        }

        public AuthorizationResult Check(IUser user)
        {
            if (user.IsInRole(Role.OrganisationRegistryBeheerder))
                return AuthorizationResult.Success();

            if ((!_isUnderVlimpersManagement || _allowOrganisationToBeUnderVlimpersManagement) &&
                user.IsOrganisatieBeheerderFor(_ovoNumber))
                return AuthorizationResult.Success();

            return AuthorizationResult.Fail(new InsufficientRights());
        }
    }
}
