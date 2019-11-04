namespace OrganisationRegistry.OrganisationClassification
{
    using System;
    using Events;
    using Infrastructure.Domain;
    using OrganisationClassificationType;

    public class OrganisationClassification: AggregateRoot
    {
        public string Name { get; private set; }
        public Guid OrganisationClassificationTypeId { get; private set; }

        private int _order;
        private string _externalKey;
        private bool _active;
        private string _organisationClassificationTypeName;

        public OrganisationClassification() { }

        public OrganisationClassification(
            OrganisationClassificationId id,
            string name,
            int order,
            string externalKey,
            bool active,
            OrganisationClassificationType organisationClassificationType)
        {
            ApplyChange(new OrganisationClassificationCreated(
                id,
                name,
                order,
                externalKey,
                active,
                organisationClassificationType.Id,
                organisationClassificationType.Name));
        }

        public void Update(string name, int order, string externalKey,
            bool active, OrganisationClassificationType organisationClassificationType)
        {
            ApplyChange(new OrganisationClassificationUpdated(
                Id,
                name, order, externalKey, active, organisationClassificationType.Id, organisationClassificationType.Name,
                Name, _order, _externalKey, _active, OrganisationClassificationTypeId, _organisationClassificationTypeName));
        }

        private void Apply(OrganisationClassificationCreated @event)
        {
            Id = @event.OrganisationClassificationId;
            Name = @event.Name;
            _order = @event.Order;
            _externalKey = @event.ExternalKey;
            _active = @event.Active;
            OrganisationClassificationTypeId = @event.OrganisationClassificationTypeId;
        }

        private void Apply(OrganisationClassificationUpdated @event)
        {
            Name = @event.Name;
            _order = @event.Order;
            _externalKey = @event.ExternalKey;
            _active = @event.Active;
            OrganisationClassificationTypeId = @event.OrganisationClassificationTypeId;
            _organisationClassificationTypeName = @event.OrganisationClassificationTypeName;
        }
    }
}
