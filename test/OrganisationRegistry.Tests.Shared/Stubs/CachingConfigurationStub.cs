namespace OrganisationRegistry.Tests.Shared.Stubs;

using Infrastructure.Configuration;

public class CachingConfigurationStub : ICachingConfiguration
{
    public CachingConfigurationStub()
    {
        UserCacheSlidingExpirationInMinutes = 1;
    }

    public int UserCacheSlidingExpirationInMinutes { get; }
}