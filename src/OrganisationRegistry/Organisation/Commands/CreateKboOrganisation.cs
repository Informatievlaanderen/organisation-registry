namespace OrganisationRegistry.Organisation.Commands
{
    using Purpose;
    using System.Collections.Generic;
    using System.Security.Claims;

    public class CreateKboOrganisation : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public string Name { get; }
        public string OvoNumber { get; }
        public string ShortName { get; }
        public OrganisationId ParentOrganisationId { get; }
        public string Description { get; }
        public List<PurposeId> Purposes { get; }
        public bool ShowOnVlaamseOverheidSites { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public List<AddOrganisationKey> Keys { get; set; } = new List<AddOrganisationKey>();
        public List<AddOrganisationBankAccount> BankAccounts { get; set; } = new List<AddOrganisationBankAccount>();
        public List<AddOrganisationOrganisationClassification> OrganisationClassifications { get; set; } = new List<AddOrganisationOrganisationClassification>();
        public List<AddOrganisationLocation> OrganisationLocations { get; set; } = new List<AddOrganisationLocation>();
        public ClaimsPrincipal User { get; }
        public KboNumber KboNumber { get; }

        public CreateKboOrganisation(
            OrganisationId organisationId,
            string name,
            string ovoNumber,
            string shortName,
            OrganisationId parentOrganisationId,
            string description,
            List<PurposeId> purposes,
            bool showOnVlaamseOverheidSites,
            ValidFrom validFrom,
            ValidTo validTo,
            ClaimsPrincipal user,
            KboNumber kboNumber)
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
            User = user;
            KboNumber = kboNumber;
        }
    }
}
