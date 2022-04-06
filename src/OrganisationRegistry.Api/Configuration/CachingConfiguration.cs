namespace OrganisationRegistry.Api.Configuration
{
    using OrganisationRegistry.Configuration;
    using OrganisationRegistry.Infrastructure.Configuration;

    public class CachingConfiguration : ICachingConfiguration
    {
        public CachingConfiguration(CachingConfigurationSection? cachingConfigurationSection)
        {
            DelayInSeconds =
                cachingConfigurationSection?.UserCacheSlidingExpirationInMinutes ?? 5;
        }
        public int DelayInSeconds { get; }
    }
}
