namespace OrganisationRegistry.Location.Commands
{
    public class CreateLocation : BaseCommand<LocationId>
    {
        public LocationId LocationId => Id;

        public string? CrabLocationId { get; }
        public Address Address { get; }

        public CreateLocation(LocationId locationId,
            string? crabLocationId,
            string street,
            string zipCode,
            string city,
            string country)
        {
            Id = locationId;
            CrabLocationId = crabLocationId;
            Address = new Address(
                street,
                zipCode,
                city,
                country);
        }
    }
}
