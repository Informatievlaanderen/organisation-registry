namespace OrganisationRegistry.Organisation
{
    using System.Collections.Generic;
    using Purpose;

    public class UpdateOrganisationInfo : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public string Description { get; }
        public string Name { get; }
        public Article Article { get; }
        public string ShortName { get; }
        public List<PurposeId> Purposes { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }
        public ValidFrom OperationalValidFrom { get; }
        public ValidTo OperationalValidTo { get; }
        public bool ShowOnVlaamseOverheidSites { get; }

        public UpdateOrganisationInfo(OrganisationId organisationId,
            string name,
            Article article,
            string description,
            string shortName,
            List<PurposeId> purposes,
            bool showOnVlaamseOverheidSites,
            ValidFrom validFrom,
            ValidTo validTo,
            ValidFrom operationalValidFrom,
            ValidTo operationalValidTo)
        {
            Id = organisationId;

            Name = name;
            Article = article;
            Description = description;
            ShortName = shortName;
            Purposes = purposes;
            ShowOnVlaamseOverheidSites = showOnVlaamseOverheidSites;
            ValidFrom = validFrom;
            ValidTo = validTo;
            OperationalValidFrom = operationalValidFrom;
            OperationalValidTo = operationalValidTo;
        }
    }
}
