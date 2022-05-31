namespace OrganisationRegistry.LocationType.Commands;

public class UpdateLocationType : BaseCommand<LocationTypeId>
{
    public LocationTypeId LocationTypeId => Id;

    public LocationTypeName Name { get; }

    public UpdateLocationType(
        LocationTypeId locationTypeId,
        LocationTypeName name)
    {
        Id = locationTypeId;
        Name = name;
    }
}
