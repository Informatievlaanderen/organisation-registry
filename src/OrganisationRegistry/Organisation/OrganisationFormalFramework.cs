namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationFormalFramework
    {
        public Guid OrganisationFormalFrameworkId { get; }
        public Guid FormalFrameworkId { get; }
        public string FormalFrameworkName { get; }
        public Guid ParentOrganisationId { get; }
        public string ParentOrganisationName { get; }
        public Period Validity { get; }

        public OrganisationFormalFramework(
            Guid organisationFormalFrameworkId,
            Guid formalFrameworkId,
            string formalFrameworkName,
            Guid parentOrganisationId,
            string parentOrganisationName,
            Period validity)
        {
            ParentOrganisationId = parentOrganisationId;
            ParentOrganisationName = parentOrganisationName;
            OrganisationFormalFrameworkId = organisationFormalFrameworkId;
            FormalFrameworkId = formalFrameworkId;
            FormalFrameworkName = formalFrameworkName;
            Validity = validity;
        }

        protected bool Equals(OrganisationFormalFramework other)
        {
            return OrganisationFormalFrameworkId.Equals(other.OrganisationFormalFrameworkId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((OrganisationFormalFramework)obj);
        }

        public override int GetHashCode()
        {
            return OrganisationFormalFrameworkId.GetHashCode();
        }

        public OrganisationFormalFramework WithValidity(Period period)
        {
            return new OrganisationFormalFramework(
                OrganisationFormalFrameworkId,
                FormalFrameworkId,
                FormalFrameworkName,
                ParentOrganisationId,
                ParentOrganisationName,
                period);
        }

        public OrganisationFormalFramework WithValidFrom(ValidFrom validFrom)
        {
            return WithValidity(new Period(validFrom, Validity.End));
        }

        public OrganisationFormalFramework WithValidTo(ValidTo validTo)
        {
            return WithValidity(new Period(Validity.Start, validTo));
        }
    }
}
