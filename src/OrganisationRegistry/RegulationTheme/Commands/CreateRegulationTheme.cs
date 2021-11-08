namespace OrganisationRegistry.RegulationTheme.Commands
{
    public class CreateRegulationTheme : BaseCommand<RegulationThemeId>
    {
        public RegulationThemeId RegulationThemeId => Id;

        public string Name { get; }

        public CreateRegulationTheme(
            RegulationThemeId regulationThemeId,
            string name)
        {
            Id = regulationThemeId;
            Name = name;
        }
    }
}
