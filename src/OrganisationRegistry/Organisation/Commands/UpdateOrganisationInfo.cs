namespace OrganisationRegistry.Organisation.Commands
{
    using System.Collections.Generic;
    using Purpose;

    public class UpdateOrganisationInfo : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public string Description { get; }
        public string Name { get; }
        public string ShortName { get; }
        public List<PurposeId> Purposes { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }
        public bool ShowOnVlaamseOverheidSites { get; }

        public UpdateOrganisationInfo(
            OrganisationId organisationId,
            string name,
            string description,
            string shortName,
            List<PurposeId> purposes,
            bool showOnVlaamseOverheidSites,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            Name = name;
            Description = description;
            ShortName = shortName;
            Purposes = purposes;
            ShowOnVlaamseOverheidSites = showOnVlaamseOverheidSites;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
