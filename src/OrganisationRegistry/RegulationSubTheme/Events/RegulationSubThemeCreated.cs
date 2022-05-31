namespace OrganisationRegistry.RegulationSubTheme.Events;

using System;
using Newtonsoft.Json;
using RegulationTheme;

public class RegulationSubThemeCreated : BaseEvent<RegulationSubThemeCreated>
{
    public Guid RegulationSubThemeId => Id;

    public string Name { get; }
    public Guid RegulationThemeId { get; }
    public string RegulationThemeName { get; }

    public RegulationSubThemeCreated(
        RegulationSubThemeId regulationSubThemeId,
        RegulationSubThemeName name,
        RegulationThemeId regulationThemeId,
        RegulationThemeName regulationThemeName)
    {
        Id = regulationSubThemeId;

        Name = name;
        RegulationThemeId = regulationThemeId;
        RegulationThemeName = regulationThemeName;
    }

    [JsonConstructor]
    public RegulationSubThemeCreated(
        Guid regulationSubThemeId,
        string name,
        Guid regulationThemeId,
        string regulationThemeName)
        : this(
            new RegulationSubThemeId(regulationSubThemeId),
            new RegulationSubThemeName(name),
            new RegulationThemeId(regulationThemeId),
            new RegulationThemeName(regulationThemeName)) { }
}
