namespace OrganisationRegistry.SeatType.Events
{
    using System;
    using Newtonsoft.Json;

    public class SeatTypeCreated : BaseEvent<SeatTypeCreated>
    {
        public Guid SeatTypeId => Id;

        public string Name { get; }
        public int? Order { get; }

        public SeatTypeCreated(
            SeatTypeId seatTypeId,
            string name,
            int? order)
        {
            Id = seatTypeId;

            Name = name;
            Order = order;
        }

        [JsonConstructor]
        public SeatTypeCreated(
            Guid seatTypeId,
            string name,
            int? order)
        {
            Id = seatTypeId;

            Name = name;
            Order = order;
        }
    }
}
