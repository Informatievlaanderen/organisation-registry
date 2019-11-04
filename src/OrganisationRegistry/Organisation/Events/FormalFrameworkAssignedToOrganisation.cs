namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class FormalFrameworkAssignedToOrganisation : BaseEvent<FormalFrameworkAssignedToOrganisation>
    {
        public Guid OrganisationId => Id;

        public Guid FormalFrameworkId { get; }
        public Guid ParentOrganisationId { get; }
        public Guid OrganisationFormalFrameworkId { get; }

        public FormalFrameworkAssignedToOrganisation(
            Guid organisationId,
            Guid formalFrameworkId,
            Guid parentOrganisationId,
            Guid organisationFormalFrameworkId)
        {
            Id = organisationId;

            FormalFrameworkId = formalFrameworkId;
            ParentOrganisationId = parentOrganisationId;
            OrganisationFormalFrameworkId = organisationFormalFrameworkId;
        }
    }
}
