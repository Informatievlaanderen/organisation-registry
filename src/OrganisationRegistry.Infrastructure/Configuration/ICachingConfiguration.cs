namespace OrganisationRegistry.Infrastructure.Configuration
{
    public interface ICachingConfiguration
    {
        int UserCacheSlidingExpirationInMinutes { get; }
    }
}
