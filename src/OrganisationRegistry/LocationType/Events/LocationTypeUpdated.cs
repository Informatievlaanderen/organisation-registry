namespace OrganisationRegistry.LocationType.Events
{
    using System;

    public class LocationTypeUpdated : BaseEvent<LocationTypeUpdated>
    {
        public Guid LocationTypeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public LocationTypeUpdated(
            Guid locationTypeId,
            string name,
            string previousName)
        {
            Id = locationTypeId;

            Name = name;
            PreviousName = previousName;
        }
    }
}
