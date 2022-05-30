namespace OrganisationRegistry.LabelType.Events;

using System;
using Newtonsoft.Json;

public class LabelTypeUpdated : BaseEvent<LabelTypeUpdated>
{
    public Guid LabelTypeId => Id;

    public string Name { get; }
    public string PreviousName { get; }

    public LabelTypeUpdated(
        LabelTypeId labelTypeId,
        LabelTypeName name,
        LabelTypeName previousName)
    {
        Id = labelTypeId;

        Name = name;
        PreviousName = previousName;
    }

    [JsonConstructor]
    public LabelTypeUpdated(
        Guid labelTypeId,
        string name,
        string previousName)
        : this(
            new LabelTypeId(labelTypeId),
            new LabelTypeName(name),
            new LabelTypeName(previousName)) { }
}