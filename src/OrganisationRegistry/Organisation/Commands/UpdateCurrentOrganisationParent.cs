namespace OrganisationRegistry.Organisation.Commands
{
    public class UpdateCurrentOrganisationParent : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public UpdateCurrentOrganisationParent(OrganisationId organisationId)
        {
            Id = organisationId;
        }

        protected bool Equals(UpdateCurrentOrganisationParent other)
        {
            return OrganisationId.Equals(other.OrganisationId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UpdateCurrentOrganisationParent) obj);
        }

        public override int GetHashCode()
        {
            return OrganisationId.GetHashCode();
        }
    }
}
