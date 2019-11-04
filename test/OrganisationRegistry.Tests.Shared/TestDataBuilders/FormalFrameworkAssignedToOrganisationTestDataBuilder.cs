namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using Organisation.Events;

    public class FormalFrameworkAssignedToOrganisationTestDataBuilder
    {
        public Guid OrganisationFormalFrameworkId { get; }
        public Guid OrganisationId { get; }
        public Guid FormalFrameworkId { get; }
        public Guid ParentOrganisationId { get; }

        public FormalFrameworkAssignedToOrganisationTestDataBuilder(
            Guid organisationFormalFrameworkId,
            Guid formalFrameworkId,
            Guid organisationId,
            Guid parentOrganisationId)
        {
            OrganisationFormalFrameworkId = organisationFormalFrameworkId;
            FormalFrameworkId = formalFrameworkId;
            OrganisationId = organisationId;
            ParentOrganisationId = parentOrganisationId;
        }

        public FormalFrameworkAssignedToOrganisation Build()
            => new FormalFrameworkAssignedToOrganisation(
                OrganisationId,
                FormalFrameworkId,
                ParentOrganisationId,
                OrganisationFormalFrameworkId);
    }
}
