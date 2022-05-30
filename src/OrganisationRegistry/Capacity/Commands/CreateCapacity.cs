namespace OrganisationRegistry.Capacity.Commands;

public class CreateCapacity : BaseCommand<CapacityId>
{
    public CapacityId CapacityId => Id;

    public string Name { get; }

    public CreateCapacity(
        CapacityId capacityId,
        string name)
    {
        Id = capacityId;
        Name = name;
    }
}