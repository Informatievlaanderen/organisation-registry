namespace OrganisationRegistry.SeatType.Commands
{
    public class UpdateSeatType : BaseCommand<SeatTypeId>
    {
        public SeatTypeId SeatTypeId => Id;

        public string Name { get; }
        public int? Order { get; }

        public UpdateSeatType(
            SeatTypeId seatTypeId,
            string name,
            int? order)
        {
            Id = seatTypeId;

            Name = name;
            Order = order;
        }
    }
}
