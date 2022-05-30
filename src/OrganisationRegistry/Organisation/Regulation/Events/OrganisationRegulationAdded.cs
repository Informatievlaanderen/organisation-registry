namespace OrganisationRegistry.Organisation.Events;

using System;
using Newtonsoft.Json;

public class OrganisationRegulationAdded : BaseEvent<OrganisationRegulationAdded>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationRegulationId { get; }
    public Guid? RegulationThemeId { get; }
    public string? RegulationThemeName { get; }
    public Guid? RegulationSubThemeId { get; }
    public string? RegulationSubThemeName { get; }
    public string Name { get; }
    public string? Uri { get; }
    public string? WorkRulesUrl { get; }
    public DateTime? Date { get; }
    public string? Description { get; }
    public string? DescriptionRendered { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public OrganisationRegulationAdded(Guid organisationId,
        Guid organisationRegulationId,
        Guid? regulationThemeId,
        string? regulationThemeName,
        Guid? regulationSubThemeId,
        string? regulationSubThemeName,
        string name,
        string? uri,
        string? workRulesUrl,
        DateTime? date,
        string? description,
        string? descriptionRendered,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = organisationId;

        OrganisationRegulationId = organisationRegulationId;
        RegulationThemeId = regulationThemeId;
        RegulationThemeName = regulationThemeName;
        RegulationSubThemeId = regulationSubThemeId;
        RegulationSubThemeName = regulationSubThemeName;
        Name = name;
        Uri = uri;
        WorkRulesUrl = workRulesUrl;
        Description = description;
        DescriptionRendered = descriptionRendered;
        Date = date;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    [JsonConstructor]
    [Obsolete("Used only in json deserialization")]
    // REASON: Uri field was previously named link.
    // For consistency this field was changed to Uri.
    // To allow for both legacy events with link, and new events with uri
    // we introduce a ctor accepting both, only taking the new field if the
    // legacy one is NULL.
    public OrganisationRegulationAdded(Guid organisationId,
        Guid organisationRegulationId,
        Guid? regulationThemeId,
        string? regulationThemeName,
        Guid? regulationSubThemeId,
        string? regulationSubThemeName,
        string name,
        string? uri,
        string? link,
        string? workRulesUrl,
        DateTime? date,
        string? description,
        string? descriptionRendered,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = organisationId;

        OrganisationRegulationId = organisationRegulationId;
        RegulationThemeId = regulationThemeId;
        RegulationThemeName = regulationThemeName;
        RegulationSubThemeId = regulationSubThemeId;
        RegulationSubThemeName = regulationSubThemeName;
        Name = name;
        Uri = link ?? uri;
        WorkRulesUrl = workRulesUrl;
        Description = description;
        DescriptionRendered = descriptionRendered;
        Date = date;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}