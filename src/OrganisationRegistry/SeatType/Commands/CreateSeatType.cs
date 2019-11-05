namespace OrganisationRegistry.SeatType.Commands
{
    public class CreateSeatType : BaseCommand<SeatTypeId>
    {
        public SeatTypeId SeatTypeId => Id;

        public SeatTypeName Name { get; }
        public int? Order { get; }

        public CreateSeatType(
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
