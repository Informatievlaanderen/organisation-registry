namespace OrganisationRegistry.Location
{
    using Commands;
    using Events;
    using Infrastructure.Domain;

    public class Location : AggregateRoot
    {
        private string _crabLocationId;

        private string _street;
        private string _zipCode;
        private string _city;
        private string _country;
        public string FormattedAddress { get; private set; }

        private Location() { }

        public Location(LocationId id, string crabLocationId, Address address)
        {
            ApplyChange(
                new LocationCreated(
                    id,
                    crabLocationId,
                    address.FullAddress,
                    address.Street,
                    address.ZipCode,
                    address.City,
                    address.Country));
        }

        public void Update(string crabLocationId, Address address)
        {
            ApplyChange(new LocationUpdated(
                    Id,
                    crabLocationId,
                    address.FullAddress,
                    address.Street,
                    address.ZipCode,
                    address.City,
                    address.Country,
                    _crabLocationId, FormattedAddress, _street, _zipCode, _city, _country));
        }

        private void Apply(LocationCreated @event)
        {
            Id = @event.LocationId;
            _crabLocationId = @event.CrabLocationId;
            _street = @event.Street;
            _zipCode = @event.ZipCode;
            _city = @event.City;
            _country = @event.Country;
            FormattedAddress = @event.FormattedAddress;
        }

        private void Apply(LocationUpdated @event)
        {
            _crabLocationId = @event.CrabLocationId;
            _street = @event.Street;
            _zipCode = @event.ZipCode;
            _city = @event.City;
            _country = @event.Country;
            FormattedAddress = @event.FormattedAddress;
        }
    }
}
