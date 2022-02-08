namespace OrganisationRegistry.Handling.Authorization
{
    using Infrastructure.Authorization;
    using Infrastructure.Domain;
    using Organisation;

    public interface ISecurityPolicy
    {
        public AuthenticationResult Check(IUser user, ISession session);
    }
}
