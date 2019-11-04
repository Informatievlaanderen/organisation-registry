namespace OrganisationRegistry.LocationType.Commands
{
    public class UpdateLocationType : BaseCommand<LocationTypeId>
    {
        public LocationTypeId LocationTypeId => Id;

        public string Name { get; }

        public UpdateLocationType(
            LocationTypeId locationTypeId,
            string name)
        {
            Id = locationTypeId;
            Name = name;
        }
    }
}
