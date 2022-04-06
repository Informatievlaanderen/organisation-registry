namespace OrganisationRegistry.Tests.Shared.Stubs
{
    using Configuration;

    public class CachingConfigurationStub : ICachingConfiguration
    {
        public CachingConfigurationStub()
        {
            UserCacheSlidingExpirationInMinutes = 1;
        }

        public int UserCacheSlidingExpirationInMinutes { get; }
    }
}
