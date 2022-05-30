namespace OrganisationRegistry.Body.Events;

using System;

public class AssignedFunctionTypeToBodySeat : BaseEvent<AssignedFunctionTypeToBodySeat>
{
    public Guid BodyId => Id;

    public Guid BodyMandateId { get; }

    public Guid BodySeatId { get; }
    public string BodySeatNumber { get; }
    public string BodySeatName { get; }

    public int? BodySeatTypeOrder { get; }

    public Guid OrganisationId { get; }
    public string OrganisationName { get; }

    public Guid FunctionTypeId { get; }
    public string FunctionTypeName { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public AssignedFunctionTypeToBodySeat(
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
        DateTime? validTo)
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
    }
}