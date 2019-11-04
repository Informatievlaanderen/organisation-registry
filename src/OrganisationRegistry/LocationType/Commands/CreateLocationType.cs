namespace OrganisationRegistry.LocationType.Commands
{
    public class CreateLocationType : BaseCommand<LocationTypeId>
    {
        public LocationTypeId LocationTypeId => Id;

        public string Name { get; }

        public CreateLocationType(
            LocationTypeId locationTypeId,
            string name)
        {
            Id = locationTypeId;
            Name = name;
        }
    }
}
