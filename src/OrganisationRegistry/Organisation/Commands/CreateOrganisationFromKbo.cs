namespace OrganisationRegistry.Organisation.Commands;

using Purpose;
using System.Collections.Generic;

public class CreateOrganisationFromKbo : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public string Name { get; }
    public string? OvoNumber { get; }
    public string? ShortName { get; }
    public Article Article { get; }
    public OrganisationId? ParentOrganisationId { get; }
    public string? Description { get; }
    public List<PurposeId>? Purposes { get; }
    public bool ShowOnVlaamseOverheidSites { get; }
    public ValidFrom ValidFrom { get; }
    public ValidFrom OperationalValidFrom { get; }
    public ValidTo OperationalValidTo { get; }

    public ValidTo ValidTo { get; }

    public List<AddOrganisationKey> Keys { get; set; } = new List<AddOrganisationKey>();
    public List<AddOrganisationBankAccount> BankAccounts { get; set; } = new List<AddOrganisationBankAccount>();
    public List<AddOrganisationOrganisationClassification> OrganisationClassifications { get; set; } = new List<AddOrganisationOrganisationClassification>();
    public List<AddOrganisationLocation> OrganisationLocations { get; set; } = new List<AddOrganisationLocation>();
    public KboNumber KboNumber { get; }

    public CreateOrganisationFromKbo(
        OrganisationId organisationId,
        string name,
        string? ovoNumber,
        string? shortName,
        Article article,
        OrganisationId? parentOrganisationId,
        string? description,
        List<PurposeId>? purposes,
        bool showOnVlaamseOverheidSites,
        ValidFrom validFrom,
        ValidTo validTo,
        KboNumber kboNumber,
        ValidFrom operationalValidFrom,
        ValidTo operationalValidTo)
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
        KboNumber = kboNumber;
        OperationalValidFrom = operationalValidFrom;
        OperationalValidTo = operationalValidTo;
        Article = article;
    }
}