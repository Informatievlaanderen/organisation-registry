namespace OrganisationRegistry.Body;

public class AssignBodyNumber : BaseCommand<BodyId>
{
    public BodyId BodyId => Id;

    public AssignBodyNumber(BodyId bodyId)
        => Id = bodyId;
}
