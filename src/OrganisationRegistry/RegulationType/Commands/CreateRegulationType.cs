namespace OrganisationRegistry.RegulationType.Commands
{
    public class CreateRegulationType : BaseCommand<RegulationTypeId>
    {
        public RegulationTypeId RegulationTypeId => Id;

        public string Name { get; }

        public CreateRegulationType(
            RegulationTypeId regulationTypeId,
            string name)
        {
            Id = regulationTypeId;
            Name = name;
        }
    }
}
