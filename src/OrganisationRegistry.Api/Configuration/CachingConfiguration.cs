namespace OrganisationRegistry.Api.Configuration
{
    using OrganisationRegistry.Configuration;
    using OrganisationRegistry.Infrastructure.Configuration;

    public class CachingConfiguration : ICachingConfiguration
    {
        public CachingConfiguration(CachingConfigurationSection? cachingConfigurationSection)
        {
            UserCacheSlidingExpirationInMinutes =
                cachingConfigurationSection?.UserCacheSlidingExpirationInMinutes ?? 5;
        }
        public int UserCacheSlidingExpirationInMinutes { get; }
    }
}
