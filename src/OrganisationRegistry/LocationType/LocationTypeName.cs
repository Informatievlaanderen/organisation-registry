namespace OrganisationRegistry.LocationType;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class LocationTypeName : StringValueObject<LocationTypeName>
{
    public LocationTypeName([JsonProperty("name")] string locationTypeName) : base(locationTypeName) { }
}
