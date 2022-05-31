namespace OrganisationRegistry.RegulationTheme.Commands;

public class CreateRegulationTheme : BaseCommand<RegulationThemeId>
{
    public RegulationThemeId RegulationThemeId => Id;

    public RegulationThemeName Name { get; }

    public CreateRegulationTheme(
        RegulationThemeId regulationThemeId,
        RegulationThemeName name)
    {
        Id = regulationThemeId;
        Name = name;
    }
}
