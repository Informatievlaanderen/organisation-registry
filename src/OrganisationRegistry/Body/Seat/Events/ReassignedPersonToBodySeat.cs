namespace OrganisationRegistry.Body.Events;

using System;
using System.Collections.Generic;

public class ReassignedPersonToBodySeat : BaseEvent<ReassignedPersonToBodySeat>
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

    public Guid PersonId { get; }
    public Guid PreviousPersonId { get; }

    public string PersonFirstName { get; }
    public string PreviousPersonFirstName { get; }

    public string PersonName { get; }
    public string PreviousPersonName { get; }

    public Dictionary<Guid, string> Contacts { get; }
    public Dictionary<Guid, string> PreviousContacts { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public ReassignedPersonToBodySeat(
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
        DateTime? validTo,
        Guid previousBodySeatId,
        string previousBodySeatNumber,
        string previousBodySeatName,
        int? previousBodySeatTypeOrder,
        Guid previousPersonId,
        string previousPersonFirstName,
        string previousPersonName,
        Dictionary<Guid, string> previousContacts,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
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

        PreviousBodySeatId = previousBodySeatId;
        PreviousBodySeatNumber = previousBodySeatNumber;
        PreviousBodySeatName = previousBodySeatName;
        PreviousBodySeatTypeOrder = previousBodySeatTypeOrder;
        PreviousPersonId = previousPersonId;
        PreviousPersonFirstName = previousPersonFirstName;
        PreviousPersonName = previousPersonName;
        PreviousContacts = previousContacts;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}