namespace OrganisationRegistry.Organisation;

using System;
using RegulationSubTheme;
using RegulationTheme;

public class UpdateOrganisationRegulation : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public Guid OrganisationRegulationId { get; }
    public RegulationThemeId RegulationThemeId { get; }
    public RegulationSubThemeId RegulationSubThemeId { get; }
    public string Name { get; }
    public string? Link { get; }
    public string? WorkRulesUrl { get; set; }
    public DateTime? Date { get; }
    public string? Description { get; }
    public string? DescriptionRendered { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }

    public UpdateOrganisationRegulation(Guid organisationRegulationId,
        OrganisationId organisationId,
        RegulationThemeId regulationThemeId,
        RegulationSubThemeId regulationSubThemeId,
        string name,
        string? link,
        string? workRulesUrl,
        DateTime? date,
        string? description,
        string? descriptionRendered,
        ValidFrom validFrom,
        ValidTo validTo)
    {
        Id = organisationId;

        OrganisationRegulationId = organisationRegulationId;
        RegulationThemeId = regulationThemeId;
        RegulationSubThemeId = regulationSubThemeId;
        Name = name;
        Link = link;
        WorkRulesUrl = workRulesUrl;
        Date = date;
        Description = description;
        DescriptionRendered = descriptionRendered;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}