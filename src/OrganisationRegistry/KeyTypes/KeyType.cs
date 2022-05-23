namespace OrganisationRegistry.KeyTypes
{
    using Events;
    using Infrastructure.Domain;

    public class KeyType : AggregateRoot
    {
        public KeyTypeName Name { get; private set; }
        public bool IsRemoved { get; private set; }

        private KeyType()
        {
            Name = new KeyTypeName(string.Empty);
        }

        public KeyType(KeyTypeId id, KeyTypeName name)
        {
            Name = new KeyTypeName(string.Empty);

            ApplyChange(
                new KeyTypeCreated(
                    id,
                    name));
        }

        public void Update(KeyTypeName name)
        {
            ApplyChange(
                new KeyTypeUpdated(
                    Id,
                    name,
                    Name));
        }

        public void Remove()
        {
            if (IsRemoved) return;

            ApplyChange(new KeyTypeRemoved(Id));
        }

        private void Apply(KeyTypeCreated @event)
        {
            Id = @event.KeyTypeId;
            Name = new KeyTypeName(@event.Name);
            IsRemoved = false;
        }

        private void Apply(KeyTypeUpdated @event)
        {
            Name = new KeyTypeName(@event.Name);
        }

        private void Apply(KeyTypeRemoved @event)
        {
            IsRemoved = true;
        }
    }
}
