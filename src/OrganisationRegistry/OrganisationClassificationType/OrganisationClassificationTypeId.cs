namespace OrganisationRegistry.OrganisationClassificationType
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class OrganisationClassificationTypeId : GuidValueObject<OrganisationClassificationTypeId>
    {
        public OrganisationClassificationTypeId([JsonProperty("id")] Guid organisationClassificationTypeId) : base(organisationClassificationTypeId) { }
    }
}
