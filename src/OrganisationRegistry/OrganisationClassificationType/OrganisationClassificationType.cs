namespace OrganisationRegistry.OrganisationClassificationType
{
    using Events;
    using Infrastructure.Domain;

    public class OrganisationClassificationType : AggregateRoot
    {
        public OrganisationClassificationTypeName Name { get; private set; }

        private OrganisationClassificationType()
        {
            Name = new OrganisationClassificationTypeName(string.Empty);
        }

        public OrganisationClassificationType(
            OrganisationClassificationTypeId id,
            OrganisationClassificationTypeName name)
        {
            Name = new OrganisationClassificationTypeName(string.Empty);

            ApplyChange(new OrganisationClassificationTypeCreated(id, name));
        }

        public void Update(OrganisationClassificationTypeName name)
        {
            var @event = new OrganisationClassificationTypeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(OrganisationClassificationTypeCreated @event)
        {
            Id = @event.OrganisationClassificationTypeId;
            Name = new OrganisationClassificationTypeName(@event.Name);
        }

        private void Apply(OrganisationClassificationTypeUpdated @event)
        {
            Name = new OrganisationClassificationTypeName(@event.Name);
        }
    }
}
