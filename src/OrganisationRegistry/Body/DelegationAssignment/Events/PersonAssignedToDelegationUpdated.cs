namespace OrganisationRegistry.Body.Events;

using System;
using System.Collections.Generic;

public class PersonAssignedToDelegationUpdated : BaseEvent<PersonAssignedToDelegationUpdated>
{
    public Guid BodyId => Id;

    public Guid BodySeatId { get; }
    public Guid BodyMandateId { get; }
    public Guid DelegationAssignmentId { get; }
    public Guid PersonId { get; }
    public string PersonFullName { get; }
    public Dictionary<Guid, string> Contacts { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public Guid PreviousPersonId { get; }
    public string PreviousPersonFullName { get; }
    public Dictionary<Guid, string> PreviousContacts { get; }
    public DateTime? PreviouslyValidFrom { get; }
    public DateTime? PreviouslyValidTo { get; }

    public PersonAssignedToDelegationUpdated(
        Guid bodyId,
        Guid bodySeatId,
        Guid bodyMandateId,
        Guid delegationAssignmentId,
        Guid personId,
        string personFullName,
        Dictionary<Guid, string> contacts,
        DateTime? validFrom,
        DateTime? validTo,
        Guid previousPersonId,
        string previousPersonFullName,
        Dictionary<Guid, string> previousContacts,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = bodyId;

        BodySeatId = bodySeatId;
        BodyMandateId = bodyMandateId;
        DelegationAssignmentId = delegationAssignmentId;

        PersonId = personId;
        PersonFullName = personFullName;
        Contacts = contacts;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousPersonId = previousPersonId;
        PreviousPersonFullName = previousPersonFullName;
        PreviousContacts = previousContacts;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}
