namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationParent : IOrganisationField, IValidityBuilder<OrganisationParent>
    {
        public Guid Id => OrganisationOrganisationParentId;
        public Guid ParentOrganisationId { get; }
        public string ParentOrganisationName { get; }
        public Guid OrganisationOrganisationParentId { get; }
        public Period Validity { get; }

        public OrganisationParent(
            Guid organisationOrganisationParentId,
            Guid parentOrganisationId,
            string parentOrganisationName,
            Period validity)
        {
            OrganisationOrganisationParentId = organisationOrganisationParentId;
            ParentOrganisationId = parentOrganisationId;
            Validity = validity;
            ParentOrganisationName = parentOrganisationName;
        }

        protected bool Equals(OrganisationParent other)
            => OrganisationOrganisationParentId.Equals(other.OrganisationOrganisationParentId);

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((OrganisationParent)obj);
        }

        public override int GetHashCode()
            => OrganisationOrganisationParentId.GetHashCode();

        public OrganisationParent WithValidity(Period period)
            => new(
                OrganisationOrganisationParentId,
                ParentOrganisationId,
                ParentOrganisationName,
                period);

        public OrganisationParent WithValidFrom(ValidFrom validFrom)
            => WithValidity(new Period(validFrom, Validity.End));

        public OrganisationParent WithValidTo(ValidTo validTo)
            => WithValidity(new Period(Validity.Start, validTo));
    }
}
