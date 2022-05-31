namespace OrganisationRegistry.Body.Events;

using System;

public class AssignedPersonClearedFromBodyMandate : BaseEvent<AssignedPersonClearedFromBodyMandate>
{
    public Guid BodyId => Id;

    public Guid BodySeatId { get; }
    public Guid BodyMandateId { get; }
    public Guid DelegationAssignmentId { get; }

    public AssignedPersonClearedFromBodyMandate(
        Guid bodyId,
        Guid bodySeatId,
        Guid bodyMandateId,
        Guid delegationAssignmentId)
    {
        Id = bodyId;

        BodySeatId = bodySeatId;
        BodyMandateId = bodyMandateId;
        DelegationAssignmentId = delegationAssignmentId;
    }
}
