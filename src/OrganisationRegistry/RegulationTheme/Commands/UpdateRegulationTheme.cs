namespace OrganisationRegistry.RegulationTheme.Commands;

public class UpdateRegulationTheme : BaseCommand<RegulationThemeId>
{
    public RegulationThemeId RegulationThemeId => Id;

    public RegulationThemeName Name { get; }

    public UpdateRegulationTheme(
        RegulationThemeId regulationThemeId,
        RegulationThemeName name)
    {
        Id = regulationThemeId;
        Name = name;
    }
}