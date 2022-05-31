namespace OrganisationRegistry.Organisation.Events;

using System;

[Obsolete("2020-01-22: No longer used in projections. Keep but don't use.")]
public class MainBuildingAssignedToOrganisation : BaseEvent<MainBuildingAssignedToOrganisation>
{
    public Guid OrganisationId => Id;

    public Guid MainBuildingId { get; }
    public Guid OrganisationBuildingId { get; }

    [Obsolete("2020-01-22: No longer used in projections. Keep but don't use.")]
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
