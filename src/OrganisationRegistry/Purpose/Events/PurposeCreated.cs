namespace OrganisationRegistry.Purpose.Events;

using System;
using Newtonsoft.Json;

public class PurposeCreated : BaseEvent<PurposeCreated>
{
    public Guid PurposeId => Id;

    public string Name { get; }

    public PurposeCreated(
        PurposeId purposeId,
        PurposeName name)
    {
        Id = purposeId;

        Name = name;
    }

    [JsonConstructor]
    public PurposeCreated(
        Guid purposeId,
        string name)
        : this(
            new PurposeId(purposeId),
            new PurposeName(name)) { }
}
