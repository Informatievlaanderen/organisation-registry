namespace OrganisationRegistry.SeatType
{
    using Events;
    using Infrastructure.Domain;

    public class SeatType : AggregateRoot
    {
        public string Name { get; private set; }

        public int? Order { get; private set; }

        private SeatType() { }

        public SeatType(SeatTypeId id, string name, int? order)
        {
            ApplyChange(new SeatTypeCreated(id, name, order));
        }

        public void Update(string name, int? order)
        {
            var @event = new SeatTypeUpdated(Id, name, order, Name, Order);
            ApplyChange(@event);
        }

        private void Apply(SeatTypeCreated @event)
        {
            Id = @event.SeatTypeId;
            Name = @event.Name;
            Order = @event.Order;
        }

        private void Apply(SeatTypeUpdated @event)
        {
            Name = @event.Name;
            Order = @event.Order;
        }
    }
}
