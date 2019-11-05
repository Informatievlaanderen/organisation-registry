namespace OrganisationRegistry.Api.Location.Responses
{
    using System;
    using SqlServer.Location;

    public class LocationResponse
    {
        public Guid Id { get; set; }

        public string CrabLocationId { get; set; }
        public string FormattedAddress { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public LocationResponse(LocationListItem projectionItem)
        {
            Id = projectionItem.Id;
            CrabLocationId = projectionItem.CrabLocationId;
            FormattedAddress = projectionItem.FormattedAddress;
            Street = projectionItem.Street;
            ZipCode = projectionItem.ZipCode;
            City = projectionItem.City;
            Country = projectionItem.Country;
        }
    }
}
