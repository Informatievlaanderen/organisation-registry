namespace OrganisationRegistry.Api
{
    using System;
    using System.Linq;
    using Configuration;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

    public class OrganisationRegistryConfiguration : IOrganisationRegistryConfiguration
    {
        private readonly ApiConfiguration _configuration;

        public Guid KboKeyTypeId => _configuration.KBO_KeyTypeId;

        public Guid KboLegalFormClassificationTypeId => _configuration.LegalFormClassificationTypeId;

        public Guid RegisteredOfficeLocationTypeId => _configuration.RegisteredOfficeLocationTypeId;

        public Guid KboV2FormalNameLabelTypeId => _configuration.KboV2FormalNameLabelTypeId;

        public Guid KboV2RegisteredOfficeLocationTypeId => _configuration.KboV2RegisteredOfficeLocationTypeId;

        public Guid KboV2LegalFormOrganisationClassificationTypeId => _configuration.KboV2LegalFormOrganisationClassificationTypeId;

        public Guid[] OrganisationCapacityTypeIdsToTerminateEndOfNextYear { get; }
        public Guid[] OrganisationClassificationTypeIdsToTerminateEndOfNextYear { get; }

        public OrganisationRegistryConfiguration(
            ApiConfiguration configuration,
            OrganisationTerminationConfiguration terminationConfiguration)
        {
            _configuration = configuration;

            OrganisationCapacityTypeIdsToTerminateEndOfNextYear =
                terminationConfiguration.OrganisationCapacityTypeIdsToTerminateEndOfNextYear?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToArray() ?? Array.Empty<Guid>();

            OrganisationClassificationTypeIdsToTerminateEndOfNextYear =
                terminationConfiguration.OrganisationClassificationTypeIdsToTerminateEndOfNextYear?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToArray() ?? Array.Empty<Guid>();
        }
    }
}
