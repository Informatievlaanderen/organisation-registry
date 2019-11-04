namespace OrganisationRegistry.LocationType
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class LocationTypeId : GuidValueObject<LocationTypeId>
    {
        public LocationTypeId([JsonProperty("id")] Guid locationTypeId) : base(locationTypeId) { }
    }
}
