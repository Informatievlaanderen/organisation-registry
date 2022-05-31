namespace OrganisationRegistry.LocationType.Commands;

public class CreateLocationType : BaseCommand<LocationTypeId>
{
    public LocationTypeId LocationTypeId => Id;

    public LocationTypeName Name { get; }

    public CreateLocationType(
        LocationTypeId locationTypeId,
        LocationTypeName name)
    {
        Id = locationTypeId;
        Name = name;
    }
}
