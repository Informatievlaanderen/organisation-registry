namespace OrganisationRegistry.BodyClassificationType.Commands;

public class CreateBodyClassificationType : BaseCommand<BodyClassificationTypeId>
{
    public BodyClassificationTypeId BodyClassificationTypeId => Id;

    public string Name { get; }

    public CreateBodyClassificationType(
        BodyClassificationTypeId bodyClassificationTypeId,
        string name)
    {
        Id = bodyClassificationTypeId;
        Name = name;
    }
}
