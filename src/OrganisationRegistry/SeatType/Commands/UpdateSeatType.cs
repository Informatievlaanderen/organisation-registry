namespace OrganisationRegistry.SeatType.Commands
{
    public class UpdateSeatType : BaseCommand<SeatTypeId>
    {
        public SeatTypeId SeatTypeId => Id;

        public SeatTypeName Name { get; }
        public int? Order { get; }

        public UpdateSeatType(
            SeatTypeId seatTypeId,
            SeatTypeName name,
            int? order)
        {
            Id = seatTypeId;

            Name = name;
            Order = order;
        }
    }
}
