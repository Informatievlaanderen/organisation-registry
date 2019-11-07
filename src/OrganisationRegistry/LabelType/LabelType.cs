namespace OrganisationRegistry.LabelType
{
    using Events;
    using Infrastructure.Domain;

    public class LabelType : AggregateRoot
    {
        public LabelTypeName Name { get; private set; }

        private LabelType() { }

        public LabelType(
            LabelTypeId id,
            LabelTypeName name)
        {
            ApplyChange(new LabelTypeCreated(
                id,
                name));
        }

        public void Update(LabelTypeName name)
        {
            ApplyChange(new LabelTypeUpdated(
                Id,
                name,
                Name));
        }

        private void Apply(LabelTypeCreated @event)
        {
            Id = @event.LabelTypeId;
            Name = new LabelTypeName(@event.Name);
        }

        private void Apply(LabelTypeUpdated @event)
        {
            Name = new LabelTypeName(@event.Name);
        }
    }
}
