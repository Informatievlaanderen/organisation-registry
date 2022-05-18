namespace OrganisationRegistry.Body.Events
{
    using System;

    public class BodySeatAdded : BaseEvent<BodySeatAdded>
    {
        public Guid BodyId => Id;

        public Guid BodySeatId { get; }
        public string Name { get; }
        public string BodySeatNumber { get; }
        public Guid SeatTypeId { get; }
        public string SeatTypeName { get; }
        public int? SeatTypeOrder { get; }
        public bool PaidSeat { get; }
        public bool EntitledToVote { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }
        public bool? SeatTypeIsEffective { get; }

        public BodySeatAdded(
            Guid bodyId,
            Guid bodySeatId,
            string name,
            string bodySeatNumber,
            Guid seatTypeId,
            string seatTypeName,
            int? seatTypeOrder,
            bool? seatTypeIsEffective,
            bool paidSeat,
            bool entitledToVote,
            DateTime? validFrom,
            DateTime? validTo)
        {
            Id = bodyId;

            BodySeatId = bodySeatId;
            Name = name;
            BodySeatNumber = bodySeatNumber;
            SeatTypeId = seatTypeId;
            SeatTypeName = seatTypeName;
            SeatTypeOrder = seatTypeOrder;
            SeatTypeIsEffective = seatTypeIsEffective;
            PaidSeat = paidSeat;
            EntitledToVote = entitledToVote;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
