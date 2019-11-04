namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationLocation
    {
        public Guid OrganisationLocationId { get; }
        public Guid OrganisationId { get; }
        public Guid LocationId { get; }
        public string FormattedAddress { get; }
        public bool IsMainLocation { get; }
        public Guid? LocationTypeId { get; }
        public string LocationTypeName { get; }
        public Period Validity { get; }

        public OrganisationLocation(
            Guid organisationLocationId,
            Guid organisationId,
            Guid locationId,
            string formattedAddress,
            bool isMainLocation,
            Guid? locationTypeId,
            string locationTypeName,
            Period validity)
        {
            OrganisationLocationId = organisationLocationId;
            OrganisationId = organisationId;
            LocationId = locationId;
            IsMainLocation = isMainLocation;
            LocationTypeId = locationTypeId;
            LocationTypeName = locationTypeName;
            Validity = validity;
            FormattedAddress = formattedAddress;
        }

        public bool IsValid(DateTime date)
        {
            return Validity.OverlapsWith(new Period(new ValidFrom(date), new ValidTo(date)));
        }

        protected bool Equals(OrganisationLocation other)
        {
            return OrganisationLocationId.Equals(other.OrganisationLocationId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((OrganisationLocation)obj);
        }

        public override int GetHashCode()
        {
            return OrganisationLocationId.GetHashCode();
        }
    }
}
