namespace OrganisationRegistry.Body.Events;

using Newtonsoft.Json;
using System;

public class BodyBodyClassificationUpdated : BaseEvent<BodyBodyClassificationUpdated>
{
    public Guid BodyId => Id;

    public Guid BodyBodyClassificationId { get; }

    public Guid BodyClassificationTypeId { get; }
    public Guid PreviousBodyClassificationTypeId { get; }

    public string BodyClassificationTypeName { get; }
    public string PreviousBodyClassificationTypeName { get; }

    public Guid BodyClassificationId { get; }
    public Guid PreviousBodyClassificationId { get; }

    public string BodyClassificationName { get; }
    public string PreviousBodyClassificationName { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    [JsonConstructor]
    public BodyBodyClassificationUpdated(
        Guid bodyId,
        Guid bodyBodyClassificationId,
        Guid bodyClassificationTypeId,
        string bodyClassificationTypeName,
        Guid bodyClassificationId,
        string bodyClassificationName,
        DateTime? validFrom,
        DateTime? validTo,
        Guid previousBodyClassificationTypeId,
        string previousBodyClassificationTypeName,
        Guid previousBodyClassificationId,
        string previousBodyClassificationName,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = bodyId;

        BodyBodyClassificationId = bodyBodyClassificationId;
        BodyClassificationTypeId = bodyClassificationTypeId;
        BodyClassificationTypeName = bodyClassificationTypeName;
        BodyClassificationId = bodyClassificationId;
        BodyClassificationName = bodyClassificationName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousBodyClassificationTypeId = previousBodyClassificationTypeId;
        PreviousBodyClassificationTypeName = previousBodyClassificationTypeName;
        PreviousBodyClassificationId = previousBodyClassificationId;
        PreviousBodyClassificationName = previousBodyClassificationName;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}