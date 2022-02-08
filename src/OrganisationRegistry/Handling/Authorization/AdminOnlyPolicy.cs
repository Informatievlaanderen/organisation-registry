namespace OrganisationRegistry.Handling.Authorization
{
    using Infrastructure.Authorization;
    using Infrastructure.Domain;
    using Organisation.Exceptions;

    class AdminOnlyPolicy : ISecurityPolicy
    {
        public AuthorizationResult Check(IUser user, ISession session)
        {
            return user.IsInRole(Role.OrganisationRegistryBeheerder) ?
                    AuthorizationResult.Success() :
                    AuthorizationResult.Fail(new InsufficientRights());
        }
    }
}
