namespace OrganisationRegistry.Organisation
{
    public class MagdaAddressWithoutValidity
    {
        public string ZipCode { get; }
        public string Street { get; }
        public string City { get; }
        public string Country { get; }

        public MagdaAddressWithoutValidity(IMagdaAddress address)
        {
            City = address.City;
            Street = address.Street;
            ZipCode = address.ZipCode;
            Country = address.Country;
        }

        protected bool Equals(MagdaAddressWithoutValidity other)
            => string.Equals(ZipCode, other.ZipCode) &&
               string.Equals(Street, other.Street) &&
               string.Equals(City, other.City) &&
               string.Equals(Country, other.Country);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MagdaAddressWithoutValidity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ZipCode != null ? ZipCode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Street != null ? Street.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (City != null ? City.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Country != null ? Country.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
