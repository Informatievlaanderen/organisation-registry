namespace OrganisationRegistry.LocationType.Events;

using System;
using Newtonsoft.Json;

public class LocationTypeUpdated : BaseEvent<LocationTypeUpdated>
{
    public Guid LocationTypeId => Id;

    public string Name { get; }
    public string PreviousName { get; }

    public LocationTypeUpdated(
        LocationTypeId locationTypeId,
        LocationTypeName name,
        LocationTypeName previousName)
    {
        Id = locationTypeId;

        Name = name;
        PreviousName = previousName;
    }

    [JsonConstructor]
    public LocationTypeUpdated(
        Guid locationTypeId,
        string name,
        string previousName)
        : this(
            new LocationTypeId(locationTypeId),
            new LocationTypeName(name),
            new LocationTypeName(previousName)) { }
}
