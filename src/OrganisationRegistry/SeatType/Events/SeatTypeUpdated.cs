namespace OrganisationRegistry.SeatType.Events;

using System;
using Newtonsoft.Json;

public class SeatTypeUpdated : BaseEvent<SeatTypeUpdated>
{
    public Guid SeatTypeId => Id;

    public string Name { get; }
    public string PreviousName { get; }
    public bool? IsEffective { get; }

    public int? Order { get; }
    public int? PreviousOrder { get; }

    public bool? WasPreviouslyEffective { get; }

    public SeatTypeUpdated(
        SeatTypeId seatTypeId,
        SeatTypeName name,
        int? order,
        bool? isEffective,
        SeatTypeName previousName,
        int? previousOrder,
        bool wasPreviouslyEffective)
    {
        Id = seatTypeId;

        Name = name;
        Order = order;

        PreviousName = previousName;
        PreviousOrder = previousOrder;
        IsEffective = isEffective;
        WasPreviouslyEffective = wasPreviouslyEffective;
    }

    [JsonConstructor]
    public SeatTypeUpdated(
        Guid seatTypeId,
        string name,
        int? order,
        bool? isEffective,
        string previousName,
        int? previousOrder,
        bool wasPreviouslyEffective)
        : this(
            new SeatTypeId(seatTypeId),
            new SeatTypeName(name),
            order,
            isEffective,
            new SeatTypeName(previousName),
            previousOrder,
            wasPreviouslyEffective)
    {
    }
}
