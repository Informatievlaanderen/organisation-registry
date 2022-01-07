namespace OrganisationRegistry.Tests.Shared
{
    using System;
    using Organisation;

    public class OrganisationRegistryConfigurationStub : IOrganisationRegistryConfiguration
    {
        public Guid KboKeyTypeId { get; set; }

        [Obsolete("Use KboV2LegalFormOrganisationClassificationTypeId instead.")]
        public Guid KboLegalFormClassificationTypeId { get; set; }

        [Obsolete("Use KboV2RegisteredOfficeLocationTypeId instead.")]
        public Guid RegisteredOfficeLocationTypeId { get; set; }

        public Guid KboV2FormalNameLabelTypeId { get; set; }

        public Guid KboV2RegisteredOfficeLocationTypeId { get; set; }

        public Guid KboV2LegalFormOrganisationClassificationTypeId { get; set; }

        public Guid[] OrganisationCapacityTypeIdsToTerminateEndOfNextYear { get; set; }
        public Guid[] OrganisationClassificationTypeIdsToTerminateEndOfNextYear { get; set; }
        public Guid[] FormalFrameworkIdsToTerminateEndOfNextYear { get; set; }
        public Guid OrafinKeyTypeId { get; }
        public Guid VlimpersKeyTypeId { get; }
        public string OrafinOvoCode { get; }
        public Guid FormalNameLabelTypeId { get; }
        public Guid FormalShortNameLabelTypeId { get; }
    }
}
