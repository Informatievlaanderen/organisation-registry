namespace OrganisationRegistry.Building.Events;

using System;

public class BuildingCreated : BaseEvent<BuildingCreated>
{
    public Guid BuildingId => Id;

    public string Name { get; }
    public int? VimId { get; }

    public BuildingCreated(
        Guid buildingId,
        string name,
        int? vimId)
    {
        Id = buildingId;
        Name = name;
        VimId = vimId;
    }
}
