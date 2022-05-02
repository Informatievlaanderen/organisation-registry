namespace OrganisationRegistry.KeyTypes.Events
{
    using System;

    public class KeyTypeRemoved : BaseEvent<KeyTypeRemoved>
    {
        public Guid KeyTypeId
            => Id;

        public KeyTypeRemoved(Guid keyTypeId)
            => Id = keyTypeId;
    }
}
