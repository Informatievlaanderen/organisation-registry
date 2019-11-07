namespace OrganisationRegistry.OrganisationClassification
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class OrganisationClassificationName : StringValueObject<OrganisationClassificationName>
    {
        public OrganisationClassificationName([JsonProperty("name")] string organisationClassificationName) : base(organisationClassificationName) { }
    }
}
