namespace OrganisationRegistry.BodyClassificationType
{
    using Events;
    using Infrastructure.Domain;

    public class BodyClassificationType : AggregateRoot
    {
        public string Name { get; private set; }

        private BodyClassificationType() { }

        public BodyClassificationType(BodyClassificationTypeId id, string name)
        {
            ApplyChange(new BodyClassificationTypeCreated(id, name));
        }

        public void Update(string name)
        {
            var @event = new BodyClassificationTypeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(BodyClassificationTypeCreated @event)
        {
            Id = @event.BodyClassificationTypeId;
            Name = @event.Name;
        }

        private void Apply(BodyClassificationTypeUpdated @event)
        {
            Name = @event.Name;
        }
    }
}
