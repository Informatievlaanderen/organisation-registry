namespace OrganisationRegistry.Body.Commands
{
    using LifecyclePhaseType;
    using Organisation;

    public class RegisterBody : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public string BodyNumber { get; }

        public string Name { get; }
        public string ShortName { get; }
        public OrganisationId OrganisationId { get; }
        public string Description { get; }

        public Period Validity { get; }
        public Period FormalValidity { get; }

        public LifecyclePhaseTypeId ActiveLifecyclePhaseTypeId { get; }
        public LifecyclePhaseTypeId InactiveLifecyclePhaseTypeId { get; }

        public RegisterBody(
            BodyId bodyId,
            string name,
            string bodyNumber,
            string shortName,
            OrganisationId organisationId,
            string description,
            Period validity,
            Period formalValidity,
            LifecyclePhaseTypeId activeLifecyclePhaseTypeId,
            LifecyclePhaseTypeId inactiveLifecyclePhaseTypeId)
        {
            Id = bodyId;

            Name = name;
            BodyNumber = bodyNumber;
            ShortName = shortName;
            OrganisationId = organisationId;
            Description = description;
            Validity = validity;
            FormalValidity = formalValidity;
            ActiveLifecyclePhaseTypeId = activeLifecyclePhaseTypeId;
            InactiveLifecyclePhaseTypeId = inactiveLifecyclePhaseTypeId;
        }
    }
}
