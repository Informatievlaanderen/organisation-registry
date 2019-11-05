namespace OrganisationRegistry.SeatType.Events
{
    using System;
    using Newtonsoft.Json;

    public class SeatTypeUpdated : BaseEvent<SeatTypeUpdated>
    {
        public Guid SeatTypeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public int? Order { get; }
        public int? PreviousOrder { get; }

        public SeatTypeUpdated(
            SeatTypeId seatTypeId,
            SeatTypeName name,
            int? order,
            SeatTypeName previousName,
            int? previousOrder)
        {
            Id = seatTypeId;

            Name = name;
            Order = order;

            PreviousName = previousName;
            PreviousOrder = previousOrder;
        }

        [JsonConstructor]
        public SeatTypeUpdated(
            Guid seatTypeId,
            string name,
            int? order,
            string previousName,
            int? previousOrder)
            : this(
                new SeatTypeId(seatTypeId),
                new SeatTypeName(name),
                order,
                new SeatTypeName(previousName),
                previousOrder) { }
    }
}
