namespace OrganisationRegistry.RegulationType
{
    using Events;
    using Infrastructure.Domain;

    public class RegulationType : AggregateRoot
    {
        public string Name { get; private set; }

        private RegulationType() { }

        public RegulationType(RegulationTypeId id, string name)
        {
            ApplyChange(new RegulationTypeCreated(id, name));
        }

        public void Update(string name)
        {
            var @event = new RegulationTypeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(RegulationTypeCreated @event)
        {
            Id = @event.RegulationTypeId;
            Name = @event.Name;
        }

        private void Apply(RegulationTypeUpdated @event)
        {
            Name = @event.Name;
        }
    }
}
