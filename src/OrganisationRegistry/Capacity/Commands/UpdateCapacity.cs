namespace OrganisationRegistry.Capacity.Commands;

public class UpdateCapacity : BaseCommand<CapacityId>
{
    public CapacityId CapacityId => Id;

    public string Name { get; }

    public UpdateCapacity(
        CapacityId capacityId,
        string name)
    {
        Id = capacityId;
        Name = name;
    }
}
