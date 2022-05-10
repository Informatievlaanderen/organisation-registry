namespace OrganisationRegistry.Api.Backoffice.Parameters.Capacity.Requests;

using System;
using OrganisationRegistry.Capacity;
using OrganisationRegistry.Capacity.Commands;

public class RemoveCapacityRequest
{
    public Guid CapacityId { get; set; }

    public RemoveCapacityRequest(Guid capacityId)
    {
        CapacityId = capacityId;
    }
}

public static class RemoveCapacityRequestMapping
{
    public static RemoveCapacity Map(RemoveCapacityRequest message)
        => new(new CapacityId(message.CapacityId));
}
