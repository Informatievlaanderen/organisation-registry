namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationRegulationUpdated : BaseEvent<OrganisationRegulationUpdated>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationRegulationId { get; }

    public Guid? RegulationThemeId { get; }
    public Guid? PreviousRegulationThemeId { get; }

    public string? RegulationThemeName { get; }
    public string? PreviousRegulationThemeName { get; }

    public Guid? RegulationSubThemeId { get; }
    public Guid? PreviousRegulationSubThemeId { get; }

    public string? RegulationSubThemeName { get; }
    public string? PreviousRegulationSubThemeName { get; }

    public string Name { get; }
    public string PreviousName { get; }

    public string? Url { get; }
    public string? PreviousUrl { get; }

    public string? WorkRulesUrl { get; }
    public string? PreviousWorkRulesUrl { get; }

    public DateTime? Date { get; }
    public DateTime? PreviousDate { get; }

    public string? Description { get; }
    public string? DescriptionRendered { get; }

    public string? PreviousDescription { get; }
    public string? PreviousDescriptionRendered { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }

    public DateTime? PreviouslyValidTo { get; }

    public OrganisationRegulationUpdated(
        Guid organisationId,
        Guid organisationRegulationId,
        Guid? regulationThemeId,
        string? regulationThemeName,
        Guid? regulationSubThemeId,
        string? regulationSubThemeName,
        string name,
        string? url,
        string? workRulesUrl,
        DateTime? date,
        string? description,
        string? descriptionRendered,
        DateTime? validFrom,
        DateTime? validTo,
        Guid? previousRegulationThemeId,
        string? previousRegulationThemeName,
        Guid? previousRegulationSubThemeId,
        string? previousRegulationSubThemeName,
        string previousName,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo,
        string? previousUrl,
        string? previousWorkRulesUrl,
        DateTime? previousDate,
        string? previousDescription,
        string? previousDescriptionRendered)
    {
        Id = organisationId;

        OrganisationRegulationId = organisationRegulationId;
        RegulationThemeId = regulationThemeId;
        RegulationThemeName = regulationThemeName;
        RegulationSubThemeId = regulationSubThemeId;
        RegulationSubThemeName = regulationSubThemeName;
        Name = name;
        Url = url;
        WorkRulesUrl = workRulesUrl;
        Date = date;
        Description = description;
        DescriptionRendered = descriptionRendered;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousRegulationThemeId = previousRegulationThemeId;
        PreviousRegulationThemeName = previousRegulationThemeName;
        PreviousRegulationSubThemeId = previousRegulationSubThemeId;
        PreviousRegulationSubThemeName = previousRegulationSubThemeName;
        PreviousName = previousName;
        PreviousUrl = previousUrl;
        PreviousWorkRulesUrl = previousWorkRulesUrl;
        PreviousDate = previousDate;
        PreviousDescription = previousDescription;
        PreviousDescriptionRendered = previousDescriptionRendered;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}