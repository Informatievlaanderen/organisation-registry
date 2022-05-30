namespace OrganisationRegistry.Capacity.Events;

using System;

public class CapacityUpdated : BaseEvent<CapacityUpdated>
{
    public Guid CapacityId => Id;

    public string Name { get; }
    public string PreviousName { get; }

    public CapacityUpdated(
        Guid capacityId,
        string name,
        string previousName)
    {
        Id = capacityId;
        Name = name;
        PreviousName = previousName;
    }
}