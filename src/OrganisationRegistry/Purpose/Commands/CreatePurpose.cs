namespace OrganisationRegistry.Purpose.Commands
{
    public class CreatePurpose : BaseCommand<PurposeId>
    {
        public PurposeId PurposeId => Id;

        public string Name { get; }

        public CreatePurpose(
            PurposeId purposeId,
            string name)
        {
            Id = purposeId;

            Name = name;
        }
    }
}
