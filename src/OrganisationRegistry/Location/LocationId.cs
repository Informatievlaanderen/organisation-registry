namespace OrganisationRegistry.Location
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class LocationId : GuidValueObject<LocationId>
    {
        public LocationId([JsonProperty("id")] Guid locationId) : base(locationId) { }
    }
}
