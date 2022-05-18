namespace OrganisationRegistry.Body
{
    using Organisation;

    public class BodyOrganisation
    {
        public OrganisationId OrganisationId { get; }
        public string OrganisationName { get; }
        public BodyOrganisationId BodyOrganisationId { get; }
        public Period Validity { get; }

        public BodyOrganisation(
            BodyOrganisationId bodyOrganisationId,
            OrganisationId organisationId,
            string organisationName,
            Period validity)
        {
            BodyOrganisationId = bodyOrganisationId;
            OrganisationId = organisationId;
            Validity = validity;
            OrganisationName = organisationName;
        }

        protected bool Equals(BodyOrganisation other)
        {
            return BodyOrganisationId.Equals(other.BodyOrganisationId);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BodyOrganisation)obj);
        }

        public override int GetHashCode()
        {
            return BodyOrganisationId.GetHashCode();
        }
    }
}
