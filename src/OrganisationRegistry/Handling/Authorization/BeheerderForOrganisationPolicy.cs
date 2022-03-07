namespace OrganisationRegistry.Handling.Authorization
{
    using Infrastructure.Authorization;
    using Organisation.Exceptions;

    public class BeheerderForOrganisationRegardlessOfVlimpersPolicy : ISecurityPolicy
    {
        private readonly bool _isUnderVlimpersManagement;
        private readonly string _ovoNumber;

        public BeheerderForOrganisationRegardlessOfVlimpersPolicy(
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

            if (user.IsOrganisatieBeheerderFor(_ovoNumber))
                return AuthorizationResult.Success();

            return AuthorizationResult.Fail(new InsufficientRights());
        }
    }
}