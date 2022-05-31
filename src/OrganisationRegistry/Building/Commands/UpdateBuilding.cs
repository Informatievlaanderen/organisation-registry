namespace OrganisationRegistry.Building.Commands;

public class UpdateBuilding : BaseCommand<BuildingId>
{
    public BuildingId BuildingId => Id;

    public string Name { get; }
    public int? VimId { get; }

    public UpdateBuilding(
        BuildingId buildingId,
        string name,
        int? vimId)
    {
        Id = buildingId;
        Name = name;
        VimId = vimId;
    }
}
