namespace OrganisationRegistry.Organisation.Commands
{
    using System.Collections.Generic;
    using Purpose;

    public class CreateOrganisation : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public string Name { get; }
        public string OvoNumber { get; }
        public string ShortName { get; }
        public Article Article { get; }
        public OrganisationId ParentOrganisationId { get; }
        public string Description { get; }
        public List<PurposeId> Purposes { get; }
        public bool ShowOnVlaamseOverheidSites { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }
        public ValidFrom OperationalValidFrom { get; }
        public ValidTo OperationalValidTo { get; }

        public CreateOrganisation(
            OrganisationId organisationId,
            string name,
            string ovoNumber,
            string shortName,
            Article article,
            OrganisationId parentOrganisationId,
            string description,
            List<PurposeId> purposes,
            bool showOnVlaamseOverheidSites,
            ValidFrom validFrom, ValidTo validTo,
            ValidFrom operationalValidFrom, ValidTo operationalValidTo)
        {
            Id = organisationId;

            Name = name;
            OvoNumber = ovoNumber;
            ShortName = shortName;
            ParentOrganisationId = parentOrganisationId;
            Description = description;
            Purposes = purposes ?? new List<PurposeId>();
            ShowOnVlaamseOverheidSites = showOnVlaamseOverheidSites;
            ValidFrom = validFrom;
            ValidTo = validTo;
            OperationalValidFrom = operationalValidFrom;
            OperationalValidTo = operationalValidTo;
            Article = article;
        }
    }
}
