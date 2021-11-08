namespace OrganisationRegistry.RegulationTheme.Commands
{
    public class UpdateRegulationTheme : BaseCommand<RegulationThemeId>
    {
        public RegulationThemeId RegulationThemeId => Id;

        public string Name { get; }

        public UpdateRegulationTheme(
            RegulationThemeId regulationThemeId,
            string name)
        {
            Id = regulationThemeId;
            Name = name;
        }
    }
}
