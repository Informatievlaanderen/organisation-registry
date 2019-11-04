namespace OrganisationRegistry.Purpose.Commands
{
    public class UpdatePurpose : BaseCommand<PurposeId>
    {
        public PurposeId PurposeId => Id;

        public string Name { get; }

        public UpdatePurpose(
            PurposeId purposeId,
            string name)
        {
            Id = purposeId;

            Name = name;
        }
    }
}
