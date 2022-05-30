namespace OrganisationRegistry.Organisation;

using System.Collections.Generic;
using Purpose;

public class UpdateOrganisationInfoNotLimitedToVlimpers : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId
        => Id;

    public string? Description { get; }
    public List<PurposeId> Purposes { get; }
    public bool ShowOnVlaamseOverheidSites { get; }


    public UpdateOrganisationInfoNotLimitedToVlimpers(
        OrganisationId organisationId,
        string? bodyDescription,
        List<PurposeId> purposes,
        bool bodyShowOnVlaamseOverheidSites)
    {
        Id = organisationId;
        Description = bodyDescription;
        Purposes = purposes;
        ShowOnVlaamseOverheidSites = bodyShowOnVlaamseOverheidSites;
    }
}