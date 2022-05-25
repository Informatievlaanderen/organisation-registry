namespace OrganisationRegistry.ContactType
{
    using Events;
    using Infrastructure.Domain;

    public class ContactType : AggregateRoot
    {
        public string Name { get; private set; }

        private ContactType()
        {
            Name = string.Empty;
        }

        public ContactType(ContactTypeId id, string name)
        {
            Name = string.Empty;

            ApplyChange(new ContactTypeCreated(id, name));
        }

        public void Update(string name)
        {
            var @event = new ContactTypeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(ContactTypeCreated @event)
        {
            Id = @event.ContactTypeId;
            Name = @event.Name;
        }

        private void Apply(ContactTypeUpdated @event)
        {
            Name = @event.Name;
        }
    }
}
