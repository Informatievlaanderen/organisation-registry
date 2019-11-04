namespace OrganisationRegistry.Body.Commands
{
    using SeatType;

    public class AddBodySeat : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public BodySeatId BodySeatId { get; }
        public string Name { get; }
        public SeatTypeId SeatTypeId { get; }
        public bool PaidSeat { get; }
        public bool EntitledToVote { get; }
        public Period Validity { get; }

        public AddBodySeat(
            BodyId bodyId,
            BodySeatId bodySeatId,
            string name,
            SeatTypeId seatTypeId,
            bool paidSeat,
            bool entitledToVote,
            Period validity)
        {
            Id = bodyId;

            BodySeatId = bodySeatId;
            Name = name;
            SeatTypeId = seatTypeId;
            PaidSeat = paidSeat;
            EntitledToVote = entitledToVote;
            Validity = validity;
        }
    }
}
