namespace OrganisationRegistry.Api.Infrastructure
{
    using System;
    using System.Linq;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Organisation;

    public class OrganisationRegistryConfiguration : IOrganisationRegistryConfiguration
    {
        private readonly ApiConfiguration _configuration;

        public Guid KboKeyTypeId => _configuration.KBO_KeyTypeId;
        public Guid OrafinKeyTypeId => _configuration.Orafin_KeyTypeId;
        public string OrafinOvoCode => _configuration.Orafin_OvoCode;

        public Guid KboLegalFormClassificationTypeId => _configuration.LegalFormClassificationTypeId;

        public Guid RegisteredOfficeLocationTypeId => _configuration.RegisteredOfficeLocationTypeId;

        public Guid KboV2FormalNameLabelTypeId => _configuration.KboV2FormalNameLabelTypeId;

        public Guid KboV2RegisteredOfficeLocationTypeId => _configuration.KboV2RegisteredOfficeLocationTypeId;

        public Guid KboV2LegalFormOrganisationClassificationTypeId => _configuration.KboV2LegalFormOrganisationClassificationTypeId;

        public Guid[] OrganisationCapacityTypeIdsToTerminateEndOfNextYear { get; }
        public Guid[] OrganisationClassificationTypeIdsToTerminateEndOfNextYear { get; }
        public Guid[] FormalFrameworkIdsToTerminateEndOfNextYear { get; }

        public OrganisationRegistryConfiguration(
            ApiConfiguration configuration,
            OrganisationTerminationConfiguration? terminationConfiguration)
        {
            _configuration = configuration;

            OrganisationCapacityTypeIdsToTerminateEndOfNextYear =
                SplitGuids(terminationConfiguration?.OrganisationCapacityTypeIdsToTerminateEndOfNextYear);

            OrganisationClassificationTypeIdsToTerminateEndOfNextYear =
                SplitGuids(terminationConfiguration?.OrganisationClassificationTypeIdsToTerminateEndOfNextYear);

            FormalFrameworkIdsToTerminateEndOfNextYear =
                SplitGuids(terminationConfiguration?.FormalFrameworkIdsToTerminateEndOfNextYear);
        }

        private static Guid[] SplitGuids(string? value)
        {
            return value?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(Guid.Parse)
                .ToArray() ?? Array.Empty<Guid>();
        }
    }
}
