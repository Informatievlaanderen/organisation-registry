namespace OrganisationRegistry.Body.Events;

using System;
using System.Collections.Generic;

public class PersonAssignedToDelegationRemoved : BaseEvent<PersonAssignedToDelegationRemoved>
{
    public Guid BodyId => Id;

    public Guid BodySeatId { get; }
    public Guid BodyMandateId { get; }
    public Guid DelegationAssignmentId { get; }

    public Guid PreviousPersonId { get; }
    public string PreviousPersonFullName { get; }
    public Dictionary<Guid, string> PreviousContacts { get; }
    public DateTime? PreviouslyValidFrom { get; }
    public DateTime? PreviouslyValidTo { get; }

    public PersonAssignedToDelegationRemoved(
        Guid bodyId,
        Guid bodySeatId,
        Guid bodyMandateId,
        Guid delegationAssignmentId,
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

        PreviousPersonId = previousPersonId;
        PreviousPersonFullName = previousPersonFullName;
        PreviousContacts = previousContacts;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}