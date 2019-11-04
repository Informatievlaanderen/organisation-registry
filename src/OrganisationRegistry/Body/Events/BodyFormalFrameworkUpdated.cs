namespace OrganisationRegistry.Body.Events
{
    using System;

    public class BodyFormalFrameworkUpdated: BaseEvent<BodyFormalFrameworkUpdated>
    {
        public Guid BodyId => Id;

        public Guid BodyFormalFrameworkId { get; }

        public Guid FormalFrameworkId { get; }
        public Guid PreviousFormalFrameworkId { get; }

        public string FormalFrameworkName { get; }
        public string PreviousFormalFrameworkName { get; }

        public DateTime? ValidFrom { get; }
        public DateTime? PreviouslyValidFrom { get; }

        public DateTime? ValidTo { get; }
        public DateTime? PreviouslyValidTo { get; }

        public BodyFormalFrameworkUpdated(
            Guid bodyId,
            Guid bodyFormalFrameworkId,
            Guid formalFrameworkId,
            string formalFrameworkName,
            DateTime? validFrom,
            DateTime? validTo,
            Guid previousFormalFrameworkId,
            string previousFormalFrameworkName,
            DateTime? previouslyValidFrom,
            DateTime? previouslyValidTo)
        {
            Id = bodyId;

            BodyFormalFrameworkId = bodyFormalFrameworkId;
            FormalFrameworkId = formalFrameworkId;
            FormalFrameworkName = formalFrameworkName;
            ValidFrom = validFrom;
            ValidTo = validTo;

            PreviousFormalFrameworkId = previousFormalFrameworkId;
            PreviousFormalFrameworkName = previousFormalFrameworkName;
            PreviouslyValidFrom = previouslyValidFrom;
            PreviouslyValidTo = previouslyValidTo;
        }
    }
}
