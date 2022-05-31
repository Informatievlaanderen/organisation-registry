namespace OrganisationRegistry.Body.Events;

using System;
using System.Collections.Generic;

public class PersonAssignedToDelegation : BaseEvent<PersonAssignedToDelegation>
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

    public PersonAssignedToDelegation(
        Guid bodyId,
        Guid bodySeatId,
        Guid bodyMandateId,
        Guid delegationAssignmentId,
        Guid personId,
        string personFullName,
        Dictionary<Guid, string> contacts,
        DateTime? validFrom,
        DateTime? validTo)
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
    }
}
