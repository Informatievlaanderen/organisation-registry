namespace OrganisationRegistry.BodyClassification.Events;

using System;
using Newtonsoft.Json;

public class BodyClassificationCreated : BaseEvent<BodyClassificationCreated>
{
    public Guid BodyClassificationId => Id;

    public string Name { get; }
    public int Order { get; }
    public bool Active { get; }
    public Guid BodyClassificationTypeId { get; }
    public string BodyClassificationTypeName { get; }

    [JsonConstructor]
    public BodyClassificationCreated(
        Guid bodyClassificationId,
        string name,
        int order,
        bool active,
        Guid bodyClassificationTypeId,
        string bodyClassificationTypeName)
    {
        Id = bodyClassificationId;
        Name = name;
        Order = order;
        Active = active;
        BodyClassificationTypeId = bodyClassificationTypeId;
        BodyClassificationTypeName = bodyClassificationTypeName;
    }
}