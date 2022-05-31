namespace OrganisationRegistry.RegulationSubTheme.Commands;

using RegulationTheme;

public class CreateRegulationSubTheme : BaseCommand<RegulationSubThemeId>
{
    public RegulationSubThemeId RegulationSubThemeId => Id;

    public RegulationSubThemeName Name { get; }
    public RegulationThemeId RegulationThemeId { get; }

    public CreateRegulationSubTheme(
        RegulationSubThemeId regulationSubThemeId,
        RegulationSubThemeName name,
        RegulationThemeId regulationThemeId)
    {
        Id = regulationSubThemeId;

        Name = name;
        RegulationThemeId = regulationThemeId;
    }
}
