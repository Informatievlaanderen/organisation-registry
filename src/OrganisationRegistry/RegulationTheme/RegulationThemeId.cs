namespace OrganisationRegistry.RegulationTheme;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class RegulationThemeId : GuidValueObject<RegulationThemeId>
{
    public RegulationThemeId([JsonProperty("id")] Guid regulationThemeId) : base(regulationThemeId) { }
}