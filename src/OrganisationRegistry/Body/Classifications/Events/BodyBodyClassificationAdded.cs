namespace OrganisationRegistry.Body.Events;

using Newtonsoft.Json;
using System;

public class BodyBodyClassificationAdded : BaseEvent<BodyBodyClassificationAdded>
{
    public Guid BodyId => Id;

    public Guid BodyBodyClassificationId { get; }
    public Guid BodyClassificationTypeId { get; }
    public string BodyClassificationTypeName { get; }
    public Guid BodyClassificationId { get; }
    public string BodyClassificationName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    [JsonConstructor]
    public BodyBodyClassificationAdded(
        Guid bodyId,
        Guid bodyBodyClassificationId,
        Guid bodyClassificationTypeId,
        string bodyClassificationTypeName,
        Guid bodyClassificationId,
        string bodyClassificationName,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = bodyId;

        BodyBodyClassificationId = bodyBodyClassificationId;
        BodyClassificationTypeId = bodyClassificationTypeId;
        BodyClassificationTypeName = bodyClassificationTypeName;
        BodyClassificationId = bodyClassificationId;
        BodyClassificationName = bodyClassificationName;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}