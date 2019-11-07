namespace OrganisationRegistry.SeatType
{
    using Events;
    using Infrastructure.Domain;

    public class SeatType : AggregateRoot
    {
        public SeatTypeName Name { get; private set; }

        public int? Order { get; private set; }

        private SeatType() { }

        public SeatType(
            SeatTypeId id,
            SeatTypeName name,
            int? order)
        {
            ApplyChange(new SeatTypeCreated(
                id,
                name,
                order));
        }

        public void Update(SeatTypeName name, int? order)
        {
            ApplyChange(new SeatTypeUpdated(
                Id,
                name,
                order,
                Name,
                Order));
        }

        private void Apply(SeatTypeCreated @event)
        {
            Id = @event.SeatTypeId;
            Name = new SeatTypeName(@event.Name);
            Order = @event.Order;
        }

        private void Apply(SeatTypeUpdated @event)
        {
            Name = new SeatTypeName(@event.Name);
            Order = @event.Order;
        }
    }
}
