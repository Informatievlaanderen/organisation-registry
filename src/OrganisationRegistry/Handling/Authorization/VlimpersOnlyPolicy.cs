namespace OrganisationRegistry.Handling.Authorization
{
    using Infrastructure.Authorization;
    using Organisation.Exceptions;

    public class VlimpersOnlyPolicy : ISecurityPolicy
    {
        private readonly bool _isUnderVlimpersManagement;

        public VlimpersOnlyPolicy(
            bool isUnderVlimpersManagement)
        {
            _isUnderVlimpersManagement = isUnderVlimpersManagement;
        }

        public AuthorizationResult Check(IUser user)
        {
            if (user.IsInRole(Role.OrganisationRegistryBeheerder))
                return AuthorizationResult.Success();

            if (_isUnderVlimpersManagement &&
                user.IsAuthorizedForVlimpersOrganisations)
                return AuthorizationResult.Success();

            return AuthorizationResult.Fail(new InsufficientRights());
        }
    }
}
