namespace OrganisationRegistry.Organisation.Events
{
    using System;

    [Obsolete("2020-01-22: No longer used in projections. Keep but don't use.")]
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
