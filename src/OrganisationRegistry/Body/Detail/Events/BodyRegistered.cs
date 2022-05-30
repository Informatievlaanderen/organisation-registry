namespace OrganisationRegistry.Body.Events;

using System;

public class BodyRegistered : BaseEvent<BodyRegistered>
{
    public Guid BodyId => Id;

    public string Name { get; }
    public string BodyNumber { get; }
    public string? ShortName { get; }
    public string? Description { get; }
    public DateTime? FormalValidFrom { get; }
    public DateTime? FormalValidTo { get; }

    public BodyRegistered(
        Guid bodyId,
        string name,
        string bodyNumber,
        string? shortName,
        string? description,
        DateTime? formalValidFrom,
        DateTime? formalValidTo)
    {
        Id = bodyId;
        Name = name;
        BodyNumber = bodyNumber;
        ShortName = shortName;
        Description = description;
        FormalValidFrom = formalValidFrom;
        FormalValidTo = formalValidTo;
    }
}