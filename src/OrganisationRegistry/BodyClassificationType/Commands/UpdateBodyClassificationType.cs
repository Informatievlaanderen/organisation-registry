namespace OrganisationRegistry.BodyClassificationType.Commands;

public class UpdateBodyClassificationType : BaseCommand<BodyClassificationTypeId>
{
    public BodyClassificationTypeId BodyClassificationTypeId => Id;

    public string Name { get; }

    public UpdateBodyClassificationType(
        BodyClassificationTypeId bodyClassificationTypeId,
        string name)
    {
        Id = bodyClassificationTypeId;
        Name = name;
    }
}
