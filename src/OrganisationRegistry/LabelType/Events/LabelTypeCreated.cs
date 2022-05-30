namespace OrganisationRegistry.LabelType.Events;

using System;
using Newtonsoft.Json;

public class LabelTypeCreated : BaseEvent<LabelTypeCreated>
{
    public Guid LabelTypeId => Id;

    public string Name { get; }

    public LabelTypeCreated(
        LabelTypeId labelTypeId,
        LabelTypeName name)
    {
        Id = labelTypeId;

        Name = name;
    }

    [JsonConstructor]
    public LabelTypeCreated(
        Guid labelTypeId,
        string name)
        : this(
            new LabelTypeId(labelTypeId),
            new LabelTypeName(name)) { }
}