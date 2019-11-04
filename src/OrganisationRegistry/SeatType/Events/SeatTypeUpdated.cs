namespace OrganisationRegistry.SeatType.Events
{
    using System;

    public class SeatTypeUpdated : BaseEvent<SeatTypeUpdated>
    {
        public Guid SeatTypeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public int? Order { get; }
        public int? PreviousOrder { get; }

        public SeatTypeUpdated(
            Guid seatTypeId,
            string name,
            int? order,
            string previousName,
            int? previousOrder)
        {
            Id = seatTypeId;

            Name = name;
            Order = order;

            PreviousName = previousName;
            PreviousOrder = previousOrder;
        }
    }
}
