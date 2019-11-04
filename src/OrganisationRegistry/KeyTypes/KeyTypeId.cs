namespace OrganisationRegistry.KeyTypes
{
    using System;

    public class KeyTypeId : GenericId<KeyTypeId>
    {
        public KeyTypeId(Guid id) : base(id) { }
    }
}
