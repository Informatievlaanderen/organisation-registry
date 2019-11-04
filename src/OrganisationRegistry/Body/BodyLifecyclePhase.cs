namespace OrganisationRegistry.Body
{
    using System;
    using LifecyclePhaseType;

    public class BodyLifecyclePhase : IEquatable<BodyLifecyclePhase>
    {
        public BodyLifecyclePhaseId BodyLifecyclePhaseId { get; }
        public LifecyclePhaseTypeId LifecyclePhaseTypeId { get; }
        public string LifecyclePhaseTypeName { get; }
        public LifecyclePhaseTypeIsRepresentativeFor LifecyclePhaseTypeIsRepresentativeFor { get; }
        public Period Validity { get; }

        public BodyLifecyclePhase(
            BodyLifecyclePhaseId bodyLifecyclePhaseId,
            LifecyclePhaseTypeId lifecyclePhaseTypeId,
            string lifecyclePhaseTypeName,
            LifecyclePhaseTypeIsRepresentativeFor lifecyclePhaseTypeIsRepresentativeFor,
            Period validity)
        {
            BodyLifecyclePhaseId = bodyLifecyclePhaseId;
            LifecyclePhaseTypeId = lifecyclePhaseTypeId;
            LifecyclePhaseTypeName = lifecyclePhaseTypeName;
            LifecyclePhaseTypeIsRepresentativeFor = lifecyclePhaseTypeIsRepresentativeFor;
            Validity = validity;
        }

        public override bool Equals(object obj) => obj is BodyLifecyclePhase && Equals((BodyLifecyclePhase)obj);
        public bool Equals(BodyLifecyclePhase other) => BodyLifecyclePhaseId == other.BodyLifecyclePhaseId;
        public override int GetHashCode() => BodyLifecyclePhaseId.GetHashCode();
    }
}
