namespace OrganisationRegistry.BodyClassification.Events;

using System;
using Newtonsoft.Json;

public class BodyClassificationUpdated : BaseEvent<BodyClassificationUpdated>
{
    public Guid BodyClassificationId => Id;

    public string Name { get; }
    public string PreviousName { get; }

    public int Order { get; }
    public int PreviousOrder { get; }

    public bool Active { get; }
    public bool PreviousActive { get; }

    public Guid BodyClassificationTypeId { get; }
    public Guid PreviousBodyClassificationTypeId { get; }

    public string BodyClassificationTypeName { get; }
    public string PreviousBodyClassificationTypeName { get; }

    [JsonConstructor]
    public BodyClassificationUpdated(
        Guid bodyClassificationId,
        string name,
        int order,
        bool active,
        Guid bodyClassificationTypeId,
        string bodyClassificationTypeName,
        string previousName,
        int previousOrder,
        bool previousActive,
        Guid previousBodyClassificationTypeId,
        string previousBodyClassificationTypeName)
    {
        Id = bodyClassificationId;

        Name = name;
        Order = order;
        Active = active;
        BodyClassificationTypeId = bodyClassificationTypeId;
        BodyClassificationTypeName = bodyClassificationTypeName;

        PreviousName = previousName;
        PreviousOrder = previousOrder;
        PreviousActive = previousActive;
        PreviousBodyClassificationTypeId = previousBodyClassificationTypeId;
        PreviousBodyClassificationTypeName = previousBodyClassificationTypeName;
    }
}