namespace OrganisationRegistry.Handling.Authorization
{
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
            if (user.IsInRole(Role.OrganisationRegistryBeheerder))
                return AuthorizationResult.Success();

            if (user.IsOrganisatieBeheerderFor(_ovoNumber))
                return AuthorizationResult.Success();

            return AuthorizationResult.Fail(new InsufficientRights());
        }
    }
}
