namespace OrganisationRegistry.Handling.Authorization
{
    using Infrastructure.Authorization;

    public interface ISecurityPolicy
    {
        public AuthorizationResult Check(IUser user);
    }
}
