namespace OrganisationRegistry.Api.Configuration
{
    using System;
    using Infrastructure.OrganisationRegistryConfiguration;
    using OrganisationRegistry.Configuration;
    using OrganisationRegistry.Infrastructure.Configuration;

    public class OrganisationRegistryConfiguration : IOrganisationRegistryConfiguration
    {
        private readonly ApiConfigurationSection _configuration;

        public Guid OrafinKeyTypeId => _configuration.Orafin_KeyTypeId;
        public Guid VlimpersKeyTypeId => _configuration.Vlimpers_KeyTypeId;
        public Guid FormalNameLabelTypeId => _configuration.FormalNameLabelTypeId;
        public Guid FormalShortNameLabelTypeId => _configuration.FormalShortNameLabelTypeId;
        public string OrafinOvoCode => _configuration.Orafin_OvoCode;
        public IKboConfiguration Kbo { get; }
        public IAuthorizationConfiguration Authorization { get; }
        public OrganisationRegistryConfiguration(
            ApiConfigurationSection configuration,
            OrganisationTerminationConfigurationSection? terminationConfiguration,
            AuthorizationConfigurationSection authorizationConfiguration)
        {
            _configuration = configuration;

            Kbo = new KboConfiguration(configuration, terminationConfiguration);

            Authorization = new AuthorizationConfiguration(authorizationConfiguration);
        }
    }
}
