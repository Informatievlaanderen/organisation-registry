namespace OrganisationRegistry.Configuration
{
    public interface ICachingConfiguration
    {
        int UserCacheSlidingExpirationInMinutes { get; }
    }
}
