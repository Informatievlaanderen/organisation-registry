namespace OrganisationRegistry.Body.Events;

using System;

public class BodySeatUpdated : BaseEvent<BodySeatUpdated>
{
    public Guid BodyId => Id;

    public Guid BodySeatId { get; }

    public string Name { get; }
    public string PreviousName { get; }

    public Guid SeatTypeId { get; }
    public Guid PreviousSeatTypeId { get; }

    public string SeatTypeName { get; }
    public string PreviousSeatTypeName { get; }

    public int? SeatTypeOrder { get; }
    public int? PreviousSeatTypeOrder { get;  }
    public bool? SeatTypeIsEffective { get; }
    public bool PreviousSeatTypeWasEffective { get; }

    public bool PaidSeat { get; }
    public bool PreviouslyPaidSeat { get; }

    public bool EntitledToVote { get; }
    public bool PreviouslyEntitledToVote { get;  }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }


    public BodySeatUpdated(
        Guid bodyId,
        Guid bodySeatId,
        string name,
        Guid seatTypeId,
        string seatTypeName,
        int? seatTypeOrder,
        bool? seatTypeIsEffective,
        bool paidSeat,
        bool entitledToVote,
        DateTime? validFrom,
        DateTime? validTo,
        string previousName,
        Guid previousSeatTypeId,
        string previousSeatTypeName,
        int? previousSeatTypeOrder,
        bool previousSeatTypeWasEffective,
        bool previouslyPaidSeat,
        bool previouslyEntitledToVote,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = bodyId;

        BodySeatId = bodySeatId;
        Name = name;
        SeatTypeId = seatTypeId;
        SeatTypeName = seatTypeName;
        SeatTypeOrder = seatTypeOrder;
        SeatTypeIsEffective = seatTypeIsEffective;
        PaidSeat = paidSeat;
        EntitledToVote = entitledToVote;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousName = previousName;
        PreviousSeatTypeId = previousSeatTypeId;
        PreviousSeatTypeName = previousSeatTypeName;
        PreviousSeatTypeOrder = previousSeatTypeOrder;
        PreviousSeatTypeWasEffective = previousSeatTypeWasEffective;
        PreviouslyPaidSeat = previouslyPaidSeat;
        PreviouslyEntitledToVote = previouslyEntitledToVote;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}