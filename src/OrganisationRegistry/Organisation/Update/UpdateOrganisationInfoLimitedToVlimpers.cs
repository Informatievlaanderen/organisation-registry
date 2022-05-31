namespace OrganisationRegistry.Organisation;

public class UpdateOrganisationInfoLimitedToVlimpers : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public string Name { get; }
    public Article Article { get; }
    public string? ShortName { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }
    public ValidFrom OperationalValidFrom { get; }
    public ValidTo OperationalValidTo { get; }

    public UpdateOrganisationInfoLimitedToVlimpers(OrganisationId organisationId,
        string name,
        Article article,
        string? shortName,
        ValidFrom validFrom,
        ValidTo validTo,
        ValidFrom operationalValidFrom,
        ValidTo operationalValidTo)
    {
        Id = organisationId;

        Name = name;
        Article = article;
        ShortName = shortName;
        ValidFrom = validFrom;
        ValidTo = validTo;
        OperationalValidFrom = operationalValidFrom;
        OperationalValidTo = operationalValidTo;
    }
}
