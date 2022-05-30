namespace OrganisationRegistry.BodyClassification.Commands;

using BodyClassificationType;

public class UpdateBodyClassification : BaseCommand<BodyClassificationId>
{
    public BodyClassificationId BodyClassificationId => Id;

    public string Name { get; }
    public int Order { get; }
    public bool Active { get; }
    public BodyClassificationTypeId BodyClassificationTypeId { get; }

    public UpdateBodyClassification(
        BodyClassificationId bodyClassificationId,
        string name,
        int order,
        bool active,
        BodyClassificationTypeId bodyClassificationTypeId)
    {
        Id = bodyClassificationId;
        Name = name;
        Order = order;
        Active = active;
        BodyClassificationTypeId = bodyClassificationTypeId;
    }
}