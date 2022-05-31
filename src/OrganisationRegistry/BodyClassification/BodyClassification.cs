// ReSharper disable UnusedMember.Local
namespace OrganisationRegistry.BodyClassification;

using System;
using BodyClassificationType;
using Events;
using Infrastructure.Domain;

public class BodyClassification : AggregateRoot
{
    public string Name { get; private set; }
    private int _order;
    private bool _active;
    private Guid _bodyClassificationTypeId;
    private string _bodyClassificationTypeName;

    public BodyClassification()
    {
        _bodyClassificationTypeName = string.Empty;
        Name = string.Empty;
    }

    public BodyClassification(
        BodyClassificationId id,
        string name,
        int order,
        bool active,
        BodyClassificationType bodyClassificationType)
    {
        _bodyClassificationTypeName = string.Empty;
        Name = string.Empty;
        ApplyChange(new BodyClassificationCreated(id, name, order, active, bodyClassificationType.Id, bodyClassificationType.Name));
    }

    public void Update(string name, int order, bool active, BodyClassificationType bodyClassificationType)
    {
        ApplyChange(new BodyClassificationUpdated(
            Id,
            name, order, active, bodyClassificationType.Id, bodyClassificationType.Name,
            Name, _order, _active, _bodyClassificationTypeId, _bodyClassificationTypeName));
    }

    private void Apply(BodyClassificationCreated @event)
    {
        Id = @event.BodyClassificationId;
        Name = @event.Name;
        _order = @event.Order;
        _active = @event.Active;
        _bodyClassificationTypeId = @event.BodyClassificationTypeId;
    }

    private void Apply(BodyClassificationUpdated @event)
    {
        Name = @event.Name;
        _order = @event.Order;
        _active = @event.Active;
        _bodyClassificationTypeId = @event.BodyClassificationTypeId;
        _bodyClassificationTypeName = @event.BodyClassificationTypeName;
    }
}
