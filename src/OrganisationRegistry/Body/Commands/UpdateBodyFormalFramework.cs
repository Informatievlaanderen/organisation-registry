namespace OrganisationRegistry.Body.Commands
{
    using FormalFramework;

    public class UpdateBodyFormalFramework : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public BodyFormalFrameworkId BodyFormalFrameworkId { get; }
        public FormalFrameworkId FormalFrameworkId { get; }
        public Period Validity { get; }

        public UpdateBodyFormalFramework(
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
}
