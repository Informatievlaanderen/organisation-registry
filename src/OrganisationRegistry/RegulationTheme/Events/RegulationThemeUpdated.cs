namespace OrganisationRegistry.RegulationTheme.Events;

using System;
using Newtonsoft.Json;

public class RegulationThemeUpdated : BaseEvent<RegulationThemeUpdated>
{
    public Guid RegulationThemeId => Id;

    public string Name { get; }
    public string PreviousName { get; }

    public RegulationThemeUpdated(
        RegulationThemeId regulationThemeId,
        RegulationThemeName name,
        RegulationThemeName previousName)
    {
        Id = regulationThemeId;

        Name = name;
        PreviousName = previousName;
    }

    [JsonConstructor]
    public RegulationThemeUpdated(
        Guid regulationThemeId,
        string name,
        string previousName)
        : this(
            new RegulationThemeId(regulationThemeId),
            new RegulationThemeName(name),
            new RegulationThemeName(previousName))
    {
    }
}
