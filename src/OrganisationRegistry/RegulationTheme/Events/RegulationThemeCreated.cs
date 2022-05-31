namespace OrganisationRegistry.RegulationTheme.Events;

using System;
using Newtonsoft.Json;

public class RegulationThemeCreated : BaseEvent<RegulationThemeCreated>
{
    public Guid RegulationThemeId => Id;

    public string Name { get; }

    public RegulationThemeCreated(
        RegulationThemeId regulationThemeId,
        RegulationThemeName name)
    {
        Id = regulationThemeId;
        Name = name;
    }

    [JsonConstructor]
    public RegulationThemeCreated(
        Guid regulationThemeId,
        string name)
        : this(
            new RegulationThemeId(regulationThemeId),
            new RegulationThemeName(name))
    {
    }
}
