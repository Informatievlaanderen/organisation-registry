namespace OrganisationRegistry.SeatType;

using Events;
using Infrastructure.Domain;

public class SeatType : AggregateRoot
{
    public SeatTypeName Name { get; private set; }

    public int? Order { get; private set; }
    public bool IsEffective { get; private set; }

    public SeatType()
    {
        Name = new SeatTypeName(string.Empty);
    }

    public SeatType(
        SeatTypeId id,
        SeatTypeName name,
        int? order,
        bool isEffective)
    {
        Name = new SeatTypeName(string.Empty);

        ApplyChange(new SeatTypeCreated(
            id,
            name,
            order,
            isEffective));
    }

    public void Update(SeatTypeName name, int? order, bool isEffective)
    {
        ApplyChange(new SeatTypeUpdated(
            Id,
            name,
            order,
            isEffective,
            Name,
            Order,
            IsEffective
        ));
    }

    private void Apply(SeatTypeCreated @event)
    {
        Id = @event.SeatTypeId;
        Name = new SeatTypeName(@event.Name);
        Order = @event.Order;
        IsEffective = @event.IsEffective ?? true;
    }

    private void Apply(SeatTypeUpdated @event)
    {
        Name = new SeatTypeName(@event.Name);
        Order = @event.Order;
        IsEffective = @event.IsEffective ?? true;
    }
}