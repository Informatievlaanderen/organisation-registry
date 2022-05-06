namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationShowOnVlaamseOverheidSitesUpdated : BaseEvent<OrganisationShowOnVlaamseOverheidSitesUpdated>
{
    public Guid OrganisationId => Id;
    public bool ShowOnVlaamseOverheidSites { get; }


    public OrganisationShowOnVlaamseOverheidSitesUpdated(
        Guid organisationId,
        bool showOnVlaamseOverheidSites)
    {
        Id = organisationId;

        ShowOnVlaamseOverheidSites = showOnVlaamseOverheidSites;
    }
}