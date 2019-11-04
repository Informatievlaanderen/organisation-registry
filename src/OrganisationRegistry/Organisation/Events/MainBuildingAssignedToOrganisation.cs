namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class MainBuildingAssignedToOrganisation : BaseEvent<MainBuildingAssignedToOrganisation>
    {
        public Guid OrganisationId => Id;

        public Guid MainBuildingId { get; }
        public Guid OrganisationBuildingId { get; }

        public MainBuildingAssignedToOrganisation(
            Guid organisationId,
            Guid mainBuildingId,
            Guid organisationBuildingId)
        {
            Id = organisationId;

            MainBuildingId = mainBuildingId;
            OrganisationBuildingId = organisationBuildingId;
        }
    }
}
