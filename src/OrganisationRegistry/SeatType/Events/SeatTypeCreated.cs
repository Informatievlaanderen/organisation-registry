namespace OrganisationRegistry.SeatType.Events;

using System;
using Newtonsoft.Json;

public class SeatTypeCreated : BaseEvent<SeatTypeCreated>
{
    public Guid SeatTypeId => Id;

    public string Name { get; }
    public int? Order { get; }

    public bool? IsEffective { get; }

    public SeatTypeCreated(
        SeatTypeId seatTypeId,
        SeatTypeName name,
        int? order,
        bool? isEffective)
    {
        Id = seatTypeId;

        Name = name;
        Order = order;
        IsEffective = isEffective;
    }

    [JsonConstructor]
    public SeatTypeCreated(
        Guid seatTypeId,
        string name,
        int? order,
        bool? isEffective)
        : this(
            new SeatTypeId(seatTypeId),
            new SeatTypeName(name),
            order, 
            isEffective) { }
}
