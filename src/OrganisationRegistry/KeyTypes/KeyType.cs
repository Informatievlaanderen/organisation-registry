namespace OrganisationRegistry.KeyTypes
{
    using Events;
    using Infrastructure.Domain;

    public class KeyType : AggregateRoot
    {
        public string Name { get; private set; }

        private KeyType() { }

        public KeyType(KeyTypeId id, string name)
        {
            ApplyChange(new KeyTypeCreated(id, name));
        }

        public void Update(string name)
        {
            var @event = new KeyTypeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(KeyTypeCreated @event)
        {
            Id = @event.KeyTypeId;
            Name = @event.Name;
        }

        private void Apply(KeyTypeUpdated @event)
        {
            Name = @event.Name;
        }
    }
}
