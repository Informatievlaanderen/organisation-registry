namespace OrganisationRegistry.Location.Events
{
    using System;

    public class LocationCreated : BaseEvent<LocationCreated>
    {
        public Guid LocationId => Id;

        public string CrabLocationId { get; set; }

        public string FormattedAddress { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public LocationCreated(
            Guid locationId,
            string crabLocationId,
            string formattedAddress,
            string street,
            string zipCode,
            string city,
            string country)
        {
            Id = locationId;

            CrabLocationId = crabLocationId;
            FormattedAddress = formattedAddress;
            Street = street;
            ZipCode = zipCode;
            City = city;
            Country = country;
        }
    }
}
