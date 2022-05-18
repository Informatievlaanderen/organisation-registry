namespace OrganisationRegistry.Body;

using LifecyclePhaseType;

public class UpdateBodyLifecyclePhase : BaseCommand<BodyId>
{
    public BodyId BodyId => Id;

    public BodyLifecyclePhaseId BodyLifecyclePhaseId { get; }
    public LifecyclePhaseTypeId LifecyclePhaseTypeId { get; }
    public Period Validity { get; }

    public UpdateBodyLifecyclePhase(
        BodyId bodyId,
        BodyLifecyclePhaseId bodyLifecyclePhaseId,
        LifecyclePhaseTypeId lifecyclePhaseTypeId,
        Period validity)
    {
        Id = bodyId;

        BodyLifecyclePhaseId = bodyLifecyclePhaseId;
        LifecyclePhaseTypeId = lifecyclePhaseTypeId;
        Validity = validity;
    }
}
