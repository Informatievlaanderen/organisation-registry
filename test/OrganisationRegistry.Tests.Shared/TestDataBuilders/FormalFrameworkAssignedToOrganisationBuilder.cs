namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using Organisation.Events;

    public class FormalFrameworkAssignedToOrganisationBuilder
    {
        public Guid OrganisationFormalFrameworkId { get; }
        public Guid OrganisationId { get; }
        public Guid FormalFrameworkId { get; }
        public Guid ParentOrganisationId { get; }

        public FormalFrameworkAssignedToOrganisationBuilder(
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
            => new(
                OrganisationId,
                FormalFrameworkId,
                ParentOrganisationId,
                OrganisationFormalFrameworkId);

        public static implicit operator FormalFrameworkAssignedToOrganisation(FormalFrameworkAssignedToOrganisationBuilder builder)
            => builder.Build();
    }
}
