namespace OrganisationRegistry.LocationType.Events
{
    using System;
    using Newtonsoft.Json;

    public class LocationTypeCreated : BaseEvent<LocationTypeCreated>
    {
        public Guid LocationTypeId => Id;

        public string Name { get; }

        public LocationTypeCreated(
            LocationTypeId locationTypeId,
            LocationTypeName name)
        {
            Id = locationTypeId;
            Name = name;
        }

        [JsonConstructor]
        public LocationTypeCreated(
            Guid locationTypeId,
            string name)
            : this(
                new LocationTypeId(locationTypeId),
                new LocationTypeName(name)) { }
    }
}
