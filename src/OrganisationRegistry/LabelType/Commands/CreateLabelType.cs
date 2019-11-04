namespace OrganisationRegistry.LabelType.Commands
{
    public class CreateLabelType : BaseCommand<LabelTypeId>
    {
        public LabelTypeId LabelTypeId => Id;

        public string Name { get; }

        public CreateLabelType(
            LabelTypeId labelTypeId,
            string name)
        {
            Id = labelTypeId;
            Name = name;
        }
    }
}
