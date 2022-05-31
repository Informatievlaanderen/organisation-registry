namespace OrganisationRegistry.Capacity.Events;

using System;

public class CapacityCreated : BaseEvent<CapacityCreated>
{
    public Guid CapacityId => Id;

    public string Name { get; }

    public CapacityCreated(
        Guid capacityId,
        string name)
    {
        Id = capacityId;
        Name = name;
    }
}
