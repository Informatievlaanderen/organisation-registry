namespace OrganisationRegistry.RegulationTheme;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class RegulationThemeName : StringValueObject<RegulationThemeName>
{
    public RegulationThemeName([JsonProperty("name")] string regulationThemeName) : base(regulationThemeName) { }
}
