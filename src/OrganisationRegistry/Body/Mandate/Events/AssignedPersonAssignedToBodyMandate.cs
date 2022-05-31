namespace OrganisationRegistry.Body.Events;

using System;

public class AssignedPersonAssignedToBodyMandate : BaseEvent<AssignedPersonAssignedToBodyMandate>
{
    public Guid BodyId => Id;

    public string BodyName { get; }
    public Guid BodySeatId { get; }
    public Guid BodyMandateId { get; }
    public Guid DelegationAssignmentId { get; }
    public Guid PersonId { get; }
    public string PersonFullName { get; }
    public DateTime? ValidFrom { get; }

    public DateTime? ValidTo { get; }

    public AssignedPersonAssignedToBodyMandate(
        Guid bodyId,
        string bodyName,
        Guid bodySeatId,
        Guid bodyMandateId,
        Guid delegationAssignmentId,
        Guid personId,
        string personFullName,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = bodyId;

        BodyName = bodyName;
        BodySeatId = bodySeatId;
        BodyMandateId = bodyMandateId;
        DelegationAssignmentId = delegationAssignmentId;
        PersonId = personId;
        PersonFullName = personFullName;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
