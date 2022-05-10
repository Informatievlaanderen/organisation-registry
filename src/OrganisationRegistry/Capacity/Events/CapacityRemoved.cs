namespace OrganisationRegistry.Capacity.Events
{
    using System;

    public class CapacityRemoved : BaseEvent<CapacityRemoved>
    {
        public Guid CapacityId
            => Id;

        public CapacityRemoved(Guid capacityId)
            => Id = capacityId;
    }
}
