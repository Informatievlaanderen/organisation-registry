namespace OrganisationRegistry.Building.Events;

using System;

public class BuildingUpdated : BaseEvent<BuildingUpdated>
{
    public Guid BuildingId => Id;

    public string Name { get; }
    public string PreviousName { get; }

    public int? VimId { get; }
    public int? PreviousVimId { get; }

    public BuildingUpdated(
        Guid buildingId,
        string name,
        int? vimId,
        string previousName,
        int? previousVimId)
    {
        Id = buildingId;

        Name = name;
        VimId = vimId;

        PreviousName = previousName;
        PreviousVimId = previousVimId;
    }
}