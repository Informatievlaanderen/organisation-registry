namespace OrganisationRegistry.LabelType;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class LabelTypeName : StringValueObject<LabelTypeName>
{
    public LabelTypeName([JsonProperty("name")] string labelTypeName) : base(labelTypeName) { }
}
