namespace OrganisationRegistry.Body;

using FormalFramework;

public class AddBodyFormalFramework : BaseCommand<BodyId>
{
    public BodyId BodyId => Id;

    public BodyFormalFrameworkId BodyFormalFrameworkId { get; }
    public FormalFrameworkId FormalFrameworkId { get; }
    public Period Validity { get; }

    public AddBodyFormalFramework(
        BodyId bodyId,
        BodyFormalFrameworkId bodyFormalFrameworkId,
        FormalFrameworkId formalFrameworkId,
        Period validity)
    {
        Id = bodyId;

        BodyFormalFrameworkId = bodyFormalFrameworkId;
        FormalFrameworkId = formalFrameworkId;
        Validity = validity;
    }
}
