namespace OrganisationRegistry.LabelType.Commands
{
    public class UpdateLabelType : BaseCommand<LabelTypeId>
    {
        public LabelTypeId LabelTypeId => Id;

        public string Name { get; }

        public UpdateLabelType(
            LabelTypeId labelTypeId,
            string name)
        {
            Id = labelTypeId;
            Name = name;
        }
    }
}
