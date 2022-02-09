namespace OrganisationRegistry.Api.Configuration
{
    using System;
    using Infrastructure.OrganisationRegistryConfiguration;
    using OrganisationRegistry.Configuration;
    using OrganisationRegistry.Infrastructure.Configuration;

    public class KboConfiguration : IKboConfiguration
    {
        private readonly ApiConfigurationSection _configuration;

        public KboConfiguration(
            ApiConfigurationSection configuration,
            OrganisationTerminationConfigurationSection? terminationConfiguration)
        {
            _configuration = configuration;

            OrganisationCapacityTypeIdsToTerminateEndOfNextYear =
                terminationConfiguration?.OrganisationCapacityTypeIdsToTerminateEndOfNextYear.SplitGuids();

            OrganisationClassificationTypeIdsToTerminateEndOfNextYear =
                terminationConfiguration?.OrganisationClassificationTypeIdsToTerminateEndOfNextYear.SplitGuids();

            FormalFrameworkIdsToTerminateEndOfNextYear =
                terminationConfiguration?.FormalFrameworkIdsToTerminateEndOfNextYear.SplitGuids();

        }
        public Guid KboV2FormalNameLabelTypeId => _configuration.KboV2FormalNameLabelTypeId;

        public Guid KboV2RegisteredOfficeLocationTypeId => _configuration.KboV2RegisteredOfficeLocationTypeId;

        public Guid KboV2LegalFormOrganisationClassificationTypeId => _configuration.KboV2LegalFormOrganisationClassificationTypeId;

        public Guid[]? OrganisationCapacityTypeIdsToTerminateEndOfNextYear { get; }
        public Guid[]? OrganisationClassificationTypeIdsToTerminateEndOfNextYear { get; }
        public Guid[]? FormalFrameworkIdsToTerminateEndOfNextYear { get; }
    }
}
