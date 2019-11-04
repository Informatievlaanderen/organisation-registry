namespace OrganisationRegistry.Building
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class BuildingId : GuidValueObject<BuildingId>
    {
        public BuildingId([JsonProperty("id")] Guid buildingId) : base(buildingId) { }
    }
}
