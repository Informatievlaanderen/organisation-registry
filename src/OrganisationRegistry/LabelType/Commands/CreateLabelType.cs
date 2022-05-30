namespace OrganisationRegistry.LabelType.Commands;

public class CreateLabelType : BaseCommand<LabelTypeId>
{
    public LabelTypeId LabelTypeId => Id;

    public LabelTypeName Name { get; }

    public CreateLabelType(
        LabelTypeId labelTypeId,
        LabelTypeName name)
    {
        Id = labelTypeId;

        Name = name;
    }
}