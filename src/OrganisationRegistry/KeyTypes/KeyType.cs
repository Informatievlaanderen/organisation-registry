namespace OrganisationRegistry.KeyTypes
{
    using Events;
    using Infrastructure.Domain;

    public class KeyType : AggregateRoot
    {
        public KeyTypeName Name { get; private set; } = null!;
        public bool IsRemoved { get; private set; }

        private KeyType()
        {
        }

        public KeyType(KeyTypeId id, KeyTypeName name)
        {
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
