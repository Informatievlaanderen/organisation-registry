namespace OrganisationRegistry.LocationType
{
    using Events;
    using Infrastructure.Domain;

    public class LocationType : AggregateRoot
    {
        public LocationTypeName Name { get; private set; }

        private LocationType() { }

        public LocationType(
            LocationTypeId id,
            LocationTypeName name)
        {
            ApplyChange(new LocationTypeCreated(id, name));
        }

        public void Update(LocationTypeName name)
        {
            var @event = new LocationTypeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(LocationTypeCreated @event)
        {
            Id = @event.LocationTypeId;
            Name = new LocationTypeName(@event.Name);
        }

        private void Apply(LocationTypeUpdated @event)
        {
            Name = new LocationTypeName(@event.Name);
        }
    }
}
