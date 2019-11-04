namespace OrganisationRegistry.Body.Commands
{
    using LifecyclePhaseType;

    public class AddBodyLifecyclePhase : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public BodyLifecyclePhaseId BodyLifecyclePhaseId { get; }
        public LifecyclePhaseTypeId LifecyclePhaseTypeId { get; }
        public Period Validity { get; }

        public AddBodyLifecyclePhase(
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
}
