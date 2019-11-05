namespace OrganisationRegistry.Api
{
    using System;
    using Configuration;
    using OrganisationRegistry.Organisation;

    public class OrganisationRegistryConfiguration : IOrganisationRegistryConfiguration
    {
        private readonly ApiConfiguration _configuration;

        public Guid KboKeyTypeId => _configuration.KBO_KeyTypeId;

        public Guid KboLegalFormClassificationTypeId => _configuration.LegalFormClassificationTypeId;

        public Guid RegisteredOfficeLocationTypeId => _configuration.RegisteredOfficeLocationTypeId;

        public Guid KboV2FormalNameLabelTypeId => _configuration.KboV2FormalNameLabelTypeId;

        public Guid KboV2RegisteredOfficeLocationTypeId => _configuration.KboV2RegisteredOfficeLocationTypeId;

        public Guid KboV2LegalFormOrganisationClassificationTypeId => _configuration.KboV2LegalFormOrganisationClassificationTypeId;

        public OrganisationRegistryConfiguration(ApiConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
