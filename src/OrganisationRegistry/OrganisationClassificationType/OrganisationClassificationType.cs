namespace OrganisationRegistry.OrganisationClassificationType
{
    using Events;
    using Infrastructure.Domain;

    public class OrganisationClassificationType : AggregateRoot
    {
        public string Name { get; private set; }

        private OrganisationClassificationType() { }

        public OrganisationClassificationType(OrganisationClassificationTypeId id, string name)
        {
            ApplyChange(new OrganisationClassificationTypeCreated(id, name));
        }

        public void Update(string name)
        {
            var @event = new OrganisationClassificationTypeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(OrganisationClassificationTypeCreated @event)
        {
            Id = @event.OrganisationClassificationTypeId;
            Name = @event.Name;
        }

        private void Apply(OrganisationClassificationTypeUpdated @event)
        {
            Name = @event.Name;
        }
    }
}
