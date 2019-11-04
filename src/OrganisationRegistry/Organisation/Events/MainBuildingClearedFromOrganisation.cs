namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class MainBuildingClearedFromOrganisation : BaseEvent<MainBuildingClearedFromOrganisation>
    {
        public Guid OrganisationId => Id;

        public Guid MainBuildingId { get; }

        public MainBuildingClearedFromOrganisation(
            Guid organisationId,
            Guid mainBuildingId)
        {
            Id = organisationId;

            MainBuildingId = mainBuildingId;
        }
    }
}
