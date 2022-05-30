namespace OrganisationRegistry.Body.Events;

using System;
using System.Collections.Generic;

public class AssignedPersonToBodySeat : BaseEvent<AssignedPersonToBodySeat>
{
    public Guid BodyId => Id;

    public Guid BodyMandateId { get; }
    public Guid BodySeatId { get; }
    public Guid PersonId { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public string BodySeatNumber { get; }
    public string BodySeatName { get; }
    public string PersonFirstName { get; }
    public string PersonName { get; }

    public Dictionary<Guid, string> Contacts { get; }

    public int? BodySeatTypeOrder { get; }

    public AssignedPersonToBodySeat(
        Guid bodyId,
        Guid bodyMandateId,
        Guid bodySeatId,
        string bodySeatNumber,
        string bodySeatName,
        int? bodySeatTypeOrder,
        Guid personId,
        string personFirstName,
        string personName,
        Dictionary<Guid, string> contacts,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = bodyId;

        BodyMandateId = bodyMandateId;

        BodySeatId = bodySeatId;
        BodySeatNumber = bodySeatNumber;
        BodySeatName = bodySeatName;

        BodySeatTypeOrder = bodySeatTypeOrder;

        PersonId = personId;
        PersonFirstName = personFirstName;
        PersonName = personName;

        Contacts = contacts;

        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}