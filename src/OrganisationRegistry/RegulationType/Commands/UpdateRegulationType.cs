namespace OrganisationRegistry.RegulationType.Commands
{
    public class UpdateRegulationType : BaseCommand<RegulationTypeId>
    {
        public RegulationTypeId RegulationTypeId => Id;

        public string Name { get; }

        public UpdateRegulationType(
            RegulationTypeId regulationTypeId,
            string name)
        {
            Id = regulationTypeId;
            Name = name;
        }
    }
}
