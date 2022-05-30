namespace OrganisationRegistry.OrganisationClassificationType;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class OrganisationClassificationTypeName : StringValueObject<OrganisationClassificationTypeName>
{
    public OrganisationClassificationTypeName([JsonProperty("name")] string organisationClassificationTypeName) : base(organisationClassificationTypeName) { }
}