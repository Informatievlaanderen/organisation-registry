namespace OrganisationRegistry.Organisation.Events
{
    using System;

    [Obsolete("2020-01-22: No longer used in projections. Keep but don't use.")]
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
