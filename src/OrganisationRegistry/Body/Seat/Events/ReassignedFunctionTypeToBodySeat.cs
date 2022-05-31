namespace OrganisationRegistry.Body.Events;

using System;

public class ReassignedFunctionTypeToBodySeat : BaseEvent<ReassignedFunctionTypeToBodySeat>
{
    public Guid BodyId => Id;

    public Guid BodyMandateId { get; }

    public Guid BodySeatId { get; }
    public Guid PreviousBodySeatId { get; }

    public string BodySeatNumber { get; }
    public string PreviousBodySeatNumber { get; }

    public string BodySeatName { get; }
    public string PreviousBodySeatName { get; }

    public int? BodySeatTypeOrder { get; }
    public int? PreviousBodySeatTypeOrder { get; }

    public Guid OrganisationId { get; }
    public Guid PreviousOrganisationId { get; }

    public string OrganisationName { get; }
    public string PreviousOrganisationName { get; }

    public Guid FunctionTypeId { get; }
    public Guid PreviousFunctionTypeId { get; }

    public string FunctionTypeName { get; }
    public string PreviousFunctionTypeName { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public ReassignedFunctionTypeToBodySeat(
        Guid bodyId,
        Guid bodyMandateId,
        Guid bodySeatId,
        string bodySeatNumber,
        string bodySeatName,
        int? bodySeatTypeOrder,
        Guid organisationId,
        string organisationName,
        Guid functionTypeId,
        string functionTypeName,
        DateTime? validFrom,
        DateTime? validTo,
        Guid previousBodySeatId,
        string previousBodySeatNumber,
        string previousBodySeatName,
        int? previousBodySeatTypeOrder,
        Guid previousOrganisationId,
        string previousOrganisationName,
        Guid previousFunctionTypeId,
        string previousFunctionTypeName,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = bodyId;

        BodyMandateId = bodyMandateId;
        BodySeatId = bodySeatId;
        BodySeatNumber = bodySeatNumber;
        BodySeatName = bodySeatName;
        BodySeatTypeOrder = bodySeatTypeOrder;
        OrganisationId = organisationId;
        OrganisationName = organisationName;
        FunctionTypeId = functionTypeId;
        FunctionTypeName = functionTypeName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousBodySeatId = previousBodySeatId;
        PreviousBodySeatNumber = previousBodySeatNumber;
        PreviousBodySeatName = previousBodySeatName;
        PreviousBodySeatTypeOrder = previousBodySeatTypeOrder;
        PreviousOrganisationId = previousOrganisationId;
        PreviousOrganisationName = previousOrganisationName;
        PreviousFunctionTypeId = previousFunctionTypeId;
        PreviousFunctionTypeName = previousFunctionTypeName;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}
