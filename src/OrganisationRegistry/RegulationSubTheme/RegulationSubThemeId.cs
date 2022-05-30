namespace OrganisationRegistry.RegulationSubTheme;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class RegulationSubThemeId : GuidValueObject<RegulationSubThemeId>
{
    public RegulationSubThemeId([JsonProperty("id")] Guid regulationSubThemeId) : base(regulationSubThemeId) { }
}