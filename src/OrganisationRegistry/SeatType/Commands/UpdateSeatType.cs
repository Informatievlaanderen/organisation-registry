namespace OrganisationRegistry.SeatType.Commands;

public class UpdateSeatType : BaseCommand<SeatTypeId>
{
    public SeatTypeId SeatTypeId => Id;

    public SeatTypeName Name { get; }
    public int? Order { get; }
    public bool IsEffective { get; }

    public UpdateSeatType(
        SeatTypeId seatTypeId,
        SeatTypeName name,
        int? order,
        bool isEffective)
    {
        Id = seatTypeId;

        Name = name;
        Order = order;
        IsEffective = isEffective;
    }
}
