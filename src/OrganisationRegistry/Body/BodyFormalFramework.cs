namespace OrganisationRegistry.Body
{
    using System;
    using FormalFramework;

    public class BodyFormalFramework : IEquatable<BodyFormalFramework>
    {
        public BodyFormalFrameworkId BodyFormalFrameworkId { get; }
        public FormalFrameworkId FormalFrameworkId { get; }
        public string FormalFrameworkName { get; }
        public Period Validity { get; }

        public BodyFormalFramework(
            BodyFormalFrameworkId bodyFormalFrameworkId,
            FormalFrameworkId formalFrameworkId,
            string formalFrameworkName,
            Period validity)
        {
            BodyFormalFrameworkId = bodyFormalFrameworkId;
            FormalFrameworkId = formalFrameworkId;
            FormalFrameworkName = formalFrameworkName;
            Validity = validity;
        }

        public override bool Equals(object obj) => obj is BodyFormalFramework && Equals((BodyFormalFramework) obj);
        public bool Equals(BodyFormalFramework other) => BodyFormalFrameworkId == other.BodyFormalFrameworkId;
        public override int GetHashCode() => BodyFormalFrameworkId.GetHashCode();
    }
}
