namespace OrganisationRegistry.LabelType.Commands;

public class UpdateLabelType : BaseCommand<LabelTypeId>
{
    public LabelTypeId LabelTypeId => Id;

    public LabelTypeName Name { get; }

    public UpdateLabelType(
        LabelTypeId labelTypeId,
        LabelTypeName name)
    {
        Id = labelTypeId;

        Name = name;
    }
}
