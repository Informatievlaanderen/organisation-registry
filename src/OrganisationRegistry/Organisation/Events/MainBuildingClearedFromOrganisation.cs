namespace OrganisationRegistry.Organisation.Events;

using System;

[Obsolete("2020-01-22: No longer used in projections. Keep but don't use in projections (and maybe don't use in domain either).")]
public class MainBuildingClearedFromOrganisation : BaseEvent<MainBuildingClearedFromOrganisation>
{
    public Guid OrganisationId => Id;

    public Guid MainBuildingId { get; }

    [Obsolete("2020-01-22: No longer used in projections. Keep but don't use in projections (and maybe don't use in domain either).")]
    public MainBuildingClearedFromOrganisation(
        Guid organisationId,
        Guid mainBuildingId)
    {
        Id = organisationId;

        MainBuildingId = mainBuildingId;
    }
}
