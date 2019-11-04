namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class ParentClearedFromOrganisation : BaseEvent<ParentClearedFromOrganisation>
    {
        public Guid OrganisationId => Id;

        public Guid ParentOrganisationId { get; }

        public ParentClearedFromOrganisation(
            Guid organisationId,
            Guid parentOrganisationId)
        {
            Id = organisationId;

            ParentOrganisationId = parentOrganisationId;
        }
    }
}
