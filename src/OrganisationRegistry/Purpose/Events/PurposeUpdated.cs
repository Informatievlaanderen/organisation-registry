namespace OrganisationRegistry.Purpose.Events;

using System;
using Newtonsoft.Json;

public class PurposeUpdated : BaseEvent<PurposeUpdated>
{
    public Guid PurposeId => Id;

    public string Name { get; }
    public string PreviousName { get; }

    public PurposeUpdated(
        PurposeId purposeId,
        PurposeName name,
        PurposeName previousName)
    {
        Id = purposeId;

        Name = name;
        PreviousName = previousName;
    }

    [JsonConstructor]
    public PurposeUpdated(
        Guid purposeId,
        string name,
        string previousName)
        : this(
            new PurposeId(purposeId),
            new PurposeName(name),
            new PurposeName(previousName)) { }
}
