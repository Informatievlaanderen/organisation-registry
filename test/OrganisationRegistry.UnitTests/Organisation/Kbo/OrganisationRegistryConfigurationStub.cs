namespace OrganisationRegistry.UnitTests.Organisation.Kbo
{
    using System;
    using OrganisationRegistry.Organisation;

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
    }
}
