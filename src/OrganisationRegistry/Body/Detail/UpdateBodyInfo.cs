namespace OrganisationRegistry.Body;

public class UpdateBodyInfo : BaseCommand<BodyId>
{
    public BodyId BodyId => Id;

    public string Name { get; }
    public string? ShortName { get; }
    public string? Description { get; }

    public UpdateBodyInfo(
        BodyId bodyId,
        string name,
        string? shortName,
        string? description)
    {
        Id = bodyId;

        Name = name;
        ShortName = shortName;
        Description = description;
    }
}
