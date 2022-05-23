namespace OrganisationRegistry.Location.Events
{
    using System;

    public class LocationUpdated : BaseEvent<LocationUpdated>
    {
        public Guid LocationId => Id;

        public string? CrabLocationId { get; set; }
        public string? PreviousCrabLocationId { get; set; }

        public string FormattedAddress { get; set; }
        public string PreviousFormattedAddress { get; set; }

        public string Street { get; set; }
        public string PreviousStreet { get; set; }

        public string ZipCode { get; set; }
        public string PreviousZipCode { get; set; }

        public string City { get; set; }
        public string PreviousCity { get; set; }

        public string Country { get; set; }
        public string PreviousCountry { get; set; }

        public LocationUpdated(
            Guid locationId,
            string? crabLocationId,
            string formattedAddress,
            string street,
            string zipCode,
            string city,
            string country,
            string? previousCrabLocationId,
            string previousFormattedAddress,
            string previousStreet,
            string previousZipCode,
            string previousCity,
            string previousCountry)
        {
            Id = locationId;

            CrabLocationId = crabLocationId;
            FormattedAddress = formattedAddress;
            Street = street;
            ZipCode = zipCode;
            City = city;
            Country = country;

            PreviousCrabLocationId = previousCrabLocationId;
            PreviousFormattedAddress = previousFormattedAddress;
            PreviousStreet = previousStreet;
            PreviousZipCode = previousZipCode;
            PreviousCity = previousCity;
            PreviousCountry = previousCountry;
        }
    }
}
