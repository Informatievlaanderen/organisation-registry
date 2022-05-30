namespace OrganisationRegistry.Purpose;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class PurposeName : StringValueObject<PurposeName>
{
    public PurposeName([JsonProperty("name")] string purposeName) : base(purposeName) { }
}