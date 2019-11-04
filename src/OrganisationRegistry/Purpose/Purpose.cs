namespace OrganisationRegistry.Purpose
{
    using Events;
    using Infrastructure.Domain;

    public class Purpose : AggregateRoot
    {
        public string Name { get; private set; }

        private Purpose() { }

        public Purpose(PurposeId id, string name)
        {
            ApplyChange(new PurposeCreated(id, name));
        }

        public void Update(string name)
        {
            var @event = new PurposeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(PurposeCreated @event)
        {
            Id = @event.PurposeId;
            Name = @event.Name;
        }

        private void Apply(PurposeUpdated @event)
        {
            Name = @event.Name;
        }
    }
}
