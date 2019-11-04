namespace OrganisationRegistry.KeyTypes.Events
{
    using System;

    public class KeyTypeCreated : BaseEvent<KeyTypeCreated>
    {
        public Guid KeyTypeId => Id;

        public string Name { get; }

        public KeyTypeCreated(
            Guid keyTypeId,
            string name)
        {
            Id = keyTypeId;
            Name = name;
        }
    }
}
