namespace OrganisationRegistry.RegulationSubTheme.Commands;

using RegulationTheme;

public class UpdateRegulationSubTheme : BaseCommand<RegulationSubThemeId>
{
    public RegulationSubThemeId RegulationSubThemeId => Id;

    public RegulationSubThemeName Name { get; }
    public RegulationThemeId RegulationThemeId { get; }

    public UpdateRegulationSubTheme(
        RegulationSubThemeId regulationSubThemeId,
        RegulationSubThemeName name,
        RegulationThemeId regulationThemeId)
    {
        Id = regulationSubThemeId;

        Name = name;
        RegulationThemeId = regulationThemeId;
    }
}
