namespace OrganisationRegistry.Handling.Authorization
{
    using Infrastructure.Authorization;
    using Infrastructure.Domain;
    using Organisation;

    public interface ISecurityPolicy
    {
        public AuthorizationResult Check(IUser user, ISession session);
    }
}
