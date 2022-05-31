namespace OrganisationRegistry.MandateRoleType;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class MandateRoleTypeName : StringValueObject<MandateRoleTypeName>
{
    public MandateRoleTypeName([JsonProperty("name")] string mandateRoleTypeName) : base(mandateRoleTypeName) { }
}
