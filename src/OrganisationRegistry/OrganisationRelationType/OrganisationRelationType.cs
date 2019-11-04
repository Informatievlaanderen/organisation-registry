namespace OrganisationRegistry.OrganisationRelationType
{
    using Events;
    using Infrastructure.Domain;

    public class OrganisationRelationType : AggregateRoot
    {
        public string Name { get; private set; }

        public string InverseName { get; private set; }

        private OrganisationRelationType() { }

        public OrganisationRelationType(OrganisationRelationTypeId id, string name, string inverseName)
        {
            ApplyChange(new OrganisationRelationTypeCreated(id, name, inverseName));
        }

        public void Update(string name, string inverseName)
        {
            var @event = new OrganisationRelationTypeUpdated(Id, name, inverseName, Name, InverseName);
            ApplyChange(@event);
        }

        private void Apply(OrganisationRelationTypeCreated @event)
        {
            Id = @event.OrganisationRelationTypeId;
            Name = @event.Name;
            InverseName = @event.InverseName;
        }

        private void Apply(OrganisationRelationTypeUpdated @event)
        {
            Name = @event.Name;
            InverseName = @event.InverseName;
        }
    }
}
