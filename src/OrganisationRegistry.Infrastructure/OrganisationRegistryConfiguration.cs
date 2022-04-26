namespace OrganisationRegistry.Api.Configuration
{
    using System;
    using OrganisationRegistry.Configuration;
    using OrganisationRegistry.Infrastructure.Configuration;

    public class OrganisationRegistryConfiguration : IOrganisationRegistryConfiguration
    {
        private readonly ApiConfigurationSection _configuration;

        public OrganisationRegistryConfiguration(
            ApiConfigurationSection configuration,
            OrganisationTerminationConfigurationSection? terminationConfiguration,
            AuthorizationConfigurationSection? authorizationConfiguration,
            CachingConfigurationSection? cachingConfiguration,
            HostedServicesConfigurationSection? hostedServicesConfiguration)
        {
            _configuration = configuration;

            Kbo = new KboConfiguration(configuration, terminationConfiguration);
            Authorization = new AuthorizationConfiguration(authorizationConfiguration);
            Caching = new CachingConfiguration(cachingConfiguration);
            HostedServices = new HostedServicesConfiguration(hostedServicesConfiguration);
        }

        public Guid OrafinKeyTypeId
            => _configuration.Orafin_KeyTypeId;

        public Guid VlimpersKeyTypeId
            => _configuration.Vlimpers_KeyTypeId;

        public Guid FormalNameLabelTypeId
            => _configuration.FormalNameLabelTypeId;

        public Guid FormalShortNameLabelTypeId
            => _configuration.FormalShortNameLabelTypeId;

        public string OrafinOvoCode
            => _configuration.Orafin_OvoCode;

        public IKboConfiguration Kbo { get; }
        public IAuthorizationConfiguration Authorization { get; }
        public ICachingConfiguration Caching { get; }
        public IHostedServicesConfiguration HostedServices { get; }
    }
}
