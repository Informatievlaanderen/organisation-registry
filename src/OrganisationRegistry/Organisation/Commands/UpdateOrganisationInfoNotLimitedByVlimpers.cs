namespace OrganisationRegistry.Organisation.Commands
{
    using System.Collections.Generic;
    using Purpose;

    public class UpdateOrganisationInfoNotLimitedByVlimpers : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;
        public string Description { get; }
        public List<PurposeId> Purposes { get; }
        public bool ShowOnVlaamseOverheidSites { get; }


        public UpdateOrganisationInfoNotLimitedByVlimpers(OrganisationId organisationId, string bodyDescription, List<PurposeId> purposes, bool bodyShowOnVlaamseOverheidSites)
        {
            Id = organisationId;
            Description = bodyDescription;
            Purposes = purposes;
            ShowOnVlaamseOverheidSites = bodyShowOnVlaamseOverheidSites;
        }
    }
}
