namespace OrganisationRegistry.Organisation.Commands
{
    using System;

    public class UpdateRelationshipValidities : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public DateTime Date { get; }

        public UpdateRelationshipValidities(OrganisationId organisationId, DateTime date)
        {
            Id = organisationId;
            Date = date;
        }

        protected bool Equals(UpdateRelationshipValidities other)
        {
            return OrganisationId.Equals(other.OrganisationId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UpdateRelationshipValidities) obj);
        }

        public override int GetHashCode()
        {
            return OrganisationId.GetHashCode();
        }
    }
}
