namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class MainLocationAssignedToOrganisation : BaseEvent<MainLocationAssignedToOrganisation>
    {
        public Guid OrganisationId => Id;

        public Guid MainLocationId { get; }
        public Guid OrganisationLocationId { get; }

        public MainLocationAssignedToOrganisation(
            Guid organisationId,
            Guid mainLocationId,
            Guid organisationLocationId)
        {
            Id = organisationId;

            MainLocationId = mainLocationId;
            OrganisationLocationId = organisationLocationId;
        }
    }
}
