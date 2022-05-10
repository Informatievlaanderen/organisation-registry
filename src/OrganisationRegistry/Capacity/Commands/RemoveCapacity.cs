namespace OrganisationRegistry.Capacity.Commands;

public class RemoveCapacity : BaseCommand<CapacityId>
{
    public CapacityId CapacityId
        => Id;

    public RemoveCapacity(CapacityId capacityId)
    {
        Id = capacityId;
    }
}
