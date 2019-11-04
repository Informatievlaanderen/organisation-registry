namespace OrganisationRegistry.LocationType.Events
{
    using System;

    public class LocationTypeCreated : BaseEvent<LocationTypeCreated>
    {
        public Guid LocationTypeId => Id;

        public string Name { get; }

        public LocationTypeCreated(
            Guid locationTypeId,
            string name)
        {
            Id = locationTypeId;
            Name = name;
        }
    }
}
