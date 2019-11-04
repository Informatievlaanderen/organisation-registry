namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationId : GenericId<OrganisationId>
    {
        public OrganisationId(Guid id) : base(id) { }
    }
}
