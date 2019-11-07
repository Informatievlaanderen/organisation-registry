namespace OrganisationRegistry.KeyTypes.Events
{
    using System;
    using Newtonsoft.Json;

    public class KeyTypeCreated : BaseEvent<KeyTypeCreated>
    {
        public Guid KeyTypeId => Id;

        public string Name { get; }

        public KeyTypeCreated(
            KeyTypeId keyTypeId,
            KeyTypeName name)
        {
            Id = keyTypeId;
            Name = name;
        }

        [JsonConstructor]
        public KeyTypeCreated(
            Guid keyTypeId,
            string name)
            : this(
                new KeyTypeId(keyTypeId),
                new KeyTypeName(name)) { }
    }
}
