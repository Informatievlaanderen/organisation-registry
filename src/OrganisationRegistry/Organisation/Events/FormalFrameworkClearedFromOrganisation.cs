namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class FormalFrameworkClearedFromOrganisation : BaseEvent<FormalFrameworkClearedFromOrganisation>
    {
        public Guid OrganisationId => Id;

        public Guid OrganisationFormalFrameworkId { get; }
        public Guid FormalFrameworkId { get; }
        public Guid ParentOrganisationId { get; }

        public FormalFrameworkClearedFromOrganisation(
            Guid organisationFormalFrameworkId,
            Guid organisationId,
            Guid formalFrameworkId,
            Guid parentOrganisationId)
        {
            Id = organisationId;

            OrganisationFormalFrameworkId = organisationFormalFrameworkId;
            FormalFrameworkId = formalFrameworkId;
            ParentOrganisationId = parentOrganisationId;
        }
    }
}
