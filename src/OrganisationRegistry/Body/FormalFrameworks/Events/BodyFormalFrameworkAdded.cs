namespace OrganisationRegistry.Body.Events
{
    using System;

    public class BodyFormalFrameworkAdded : BaseEvent<BodyFormalFrameworkAdded>
    {
        public Guid BodyId => Id;

        public Guid BodyFormalFrameworkId { get; }
        public Guid FormalFrameworkId { get; }
        public string FormalFrameworkName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public BodyFormalFrameworkAdded(
            Guid bodyId,
            Guid bodyFormalFrameworkId,
            Guid formalFrameworkId,
            string formalFrameworkName,
            DateTime? validFrom,
            DateTime? validTo)
        {
            Id = bodyId;

            BodyFormalFrameworkId = bodyFormalFrameworkId;
            FormalFrameworkId = formalFrameworkId;
            FormalFrameworkName = formalFrameworkName;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
