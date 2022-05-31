namespace OrganisationRegistry.KeyTypes.Events;

using System;
using Newtonsoft.Json;

public class KeyTypeUpdated : BaseEvent<KeyTypeUpdated>
{
    public Guid KeyTypeId => Id;

    public string Name { get; }
    public string PreviousName { get; }

    public KeyTypeUpdated(
        KeyTypeId keyTypeId,
        KeyTypeName name,
        KeyTypeName previousName)
    {
        Id = keyTypeId;

        Name = name;
        PreviousName = previousName;
    }

    [JsonConstructor]
    public KeyTypeUpdated(
        Guid keyTypeId,
        string name,
        string previousName)
        : this(
            new KeyTypeId(keyTypeId),
            new KeyTypeName(name),
            new KeyTypeName(previousName)) { }
}
