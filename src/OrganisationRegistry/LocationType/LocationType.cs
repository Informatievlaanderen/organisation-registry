namespace OrganisationRegistry.LocationType
{
    using Events;
    using Infrastructure.Domain;

    public class LocationType : AggregateRoot
    {
        public string Name { get; private set; }

        private LocationType() { }

        public LocationType(LocationTypeId id, string name)
        {
            ApplyChange(new LocationTypeCreated(id, name));
        }

        public void Update(string name)
        {
            var @event = new LocationTypeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(LocationTypeCreated @event)
        {
            Id = @event.LocationTypeId;
            Name = @event.Name;
        }

        private void Apply(LocationTypeUpdated @event)
        {
            Name = @event.Name;
        }
    }
}
