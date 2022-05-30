namespace OrganisationRegistry.OrganisationClassification;

using System;
using Events;
using Infrastructure.Domain;
using OrganisationClassificationType;

public class OrganisationClassification : AggregateRoot
{
    public OrganisationClassificationName Name { get; private set; }
    public OrganisationClassificationTypeId OrganisationClassificationTypeId { get; private set; }

    private int _order;
    private string? _externalKey;
    private bool _active;
    private OrganisationClassificationTypeName _organisationClassificationTypeName;

    public OrganisationClassification()
    {
        Name = new OrganisationClassificationName(string.Empty);
        OrganisationClassificationTypeId = new OrganisationClassificationTypeId(Guid.Empty);

        _externalKey = string.Empty;
        _organisationClassificationTypeName = new OrganisationClassificationTypeName(string.Empty);
    }

    public OrganisationClassification(
        OrganisationClassificationId id,
        OrganisationClassificationName name,
        int order,
        string? externalKey,
        bool active,
        OrganisationClassificationType organisationClassificationType)
    {
        Name = new OrganisationClassificationName(string.Empty);
        OrganisationClassificationTypeId = new OrganisationClassificationTypeId(Guid.Empty);

        _externalKey = string.Empty;
        _organisationClassificationTypeName = new OrganisationClassificationTypeName(string.Empty);

        ApplyChange(
            new OrganisationClassificationCreated(
                id,
                name,
                order,
                externalKey,
                active,
                organisationClassificationType.Id,
                organisationClassificationType.Name));
    }

    public void Update(
        OrganisationClassificationName name,
        int order,
        string? externalKey,
        bool active,
        OrganisationClassificationType organisationClassificationType)
    {
        ApplyChange(
            new OrganisationClassificationUpdated(
                Id,
                name,
                order,
                externalKey,
                active,
                organisationClassificationType.Id,
                organisationClassificationType.Name,
                Name,
                _order,
                _externalKey,
                _active,
                OrganisationClassificationTypeId,
                _organisationClassificationTypeName));
    }

    private void Apply(OrganisationClassificationCreated @event)
    {
        Id = @event.OrganisationClassificationId;
        Name = new OrganisationClassificationName(@event.Name);
        _order = @event.Order;
        _externalKey = @event.ExternalKey;
        _active = @event.Active;
        OrganisationClassificationTypeId = new OrganisationClassificationTypeId(@event.OrganisationClassificationTypeId);
    }

    private void Apply(OrganisationClassificationUpdated @event)
    {
        Name = new OrganisationClassificationName(@event.Name);
        _order = @event.Order;
        _externalKey = @event.ExternalKey;
        _active = @event.Active;
        OrganisationClassificationTypeId = new OrganisationClassificationTypeId(@event.OrganisationClassificationTypeId);
        _organisationClassificationTypeName = new OrganisationClassificationTypeName(@event.OrganisationClassificationTypeName);
    }
}