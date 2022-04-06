namespace OrganisationRegistry.Tests.Shared.Stubs
{
    using Configuration;

    public class CachingConfigurationStub : ICachingConfiguration
    {
        public CachingConfigurationStub()
        {
            DelayInSeconds = 1;
        }

        public int DelayInSeconds { get; }
    }
}
