namespace OrganisationRegistry.LabelType
{
    using Events;
    using Infrastructure.Domain;

    public class LabelType : AggregateRoot
    {
        public string Name { get; private set; }

        private LabelType() { }

        public LabelType(LabelTypeId id, string name)
        {
            ApplyChange(new LabelTypeCreated(id, name));
        }

        public void Update(string name)
        {
            var @event = new LabelTypeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(LabelTypeCreated @event)
        {
            Id = @event.LabelTypeId;
            Name = @event.Name;
        }

        private void Apply(LabelTypeUpdated @event)
        {
            Name = @event.Name;
        }
    }
}
