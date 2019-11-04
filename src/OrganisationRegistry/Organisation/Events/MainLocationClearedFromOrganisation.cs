namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class MainLocationClearedFromOrganisation : BaseEvent<MainLocationClearedFromOrganisation>
    {
        public Guid OrganisationId => Id;

        public Guid MainLocationId { get; }

        public MainLocationClearedFromOrganisation(
            Guid organisationId,
            Guid mainLocationId)
        {
            Id = organisationId;
            MainLocationId = mainLocationId;
        }
    }
}
