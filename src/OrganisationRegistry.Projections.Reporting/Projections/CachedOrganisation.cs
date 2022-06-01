namespace OrganisationRegistry.Projections.Reporting.Projections;

using SqlServer.Reporting;
using System;

public class CachedOrganisation
{
    public Guid? OrganisationId { get; }
    public string OrganisationName { get; }
    public bool OrganisationActive { get; set; }

    private CachedOrganisation(Guid? organisationId, string organisationName, bool organisationActive = false)
    {
        OrganisationId = organisationId;
        OrganisationName = organisationName;
        OrganisationActive = organisationActive;
    }

    public static CachedOrganisation FromCache(BodySeatGenderRatioOrganisationListItem organisation)
        => new(organisation.OrganisationId, organisation.OrganisationName, organisation.OrganisationActive);

    public static CachedOrganisation Empty()
        => new(null, string.Empty);
}
