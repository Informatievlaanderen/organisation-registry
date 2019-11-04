namespace OrganisationRegistry.KeyTypes.Events
{
    using System;

    public class KeyTypeUpdated : BaseEvent<KeyTypeUpdated>
    {
        public Guid KeyTypeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public KeyTypeUpdated(
            Guid keyTypeId,
            string name,
            string previousName)
        {
            Id = keyTypeId;
            Name = name;
            PreviousName = previousName;
        }
    }
}
