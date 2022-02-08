namespace OrganisationRegistry.Handling.Authorization
{
    using Infrastructure.Authorization;
    using Infrastructure.Domain;
    using Organisation.Exceptions;

    class AdminOnlyPolicy : ISecurityPolicy
    {
        public AuthenticationResult Check(IUser user, ISession session)
        {
            return user.IsInRole(Role.OrganisationRegistryBeheerder) ?
                    AuthenticationResult.Success() :
                    AuthenticationResult.Fail(new InsufficientRights());
        }
    }
}
