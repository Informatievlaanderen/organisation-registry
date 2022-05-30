namespace OrganisationRegistry.Projections.Reporting.Projections;

using System;
using SqlServer.Reporting;

public class CachedOrganisationForBody
{
    public Guid? OrganisationId { get; }
    public string OrganisationName { get; }
    public bool OrganisationActive { get; set; }

    private CachedOrganisationForBody(Guid? organisationId, string organisationName, bool organisationActive = false)
    {
        OrganisationId = organisationId;
        OrganisationName = organisationName;
        OrganisationActive = organisationActive;
    }

    public static CachedOrganisationForBody FromCache(BodySeatGenderRatioOrganisationPerBodyListItem organisation)
        => new(organisation.OrganisationId, organisation.OrganisationName);

    public static CachedOrganisationForBody Empty()
        => new(null, string.Empty);
}