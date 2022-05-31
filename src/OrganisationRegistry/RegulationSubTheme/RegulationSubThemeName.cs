namespace OrganisationRegistry.RegulationSubTheme;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class RegulationSubThemeName : StringValueObject<RegulationSubThemeName>
{
    public RegulationSubThemeName([JsonProperty("name")] string regulationSubThemeName) : base(regulationSubThemeName) { }
}
