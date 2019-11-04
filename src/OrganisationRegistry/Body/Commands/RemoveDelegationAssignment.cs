namespace OrganisationRegistry.Body.Commands
{
    public class RemoveDelegationAssignment: BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public BodyMandateId BodyMandateId { get; }
        public BodySeatId BodySeatId { get; }
        public DelegationAssignmentId DelegationAssignmentId { get; }

        public RemoveDelegationAssignment(
            BodyMandateId bodyMandateId,
            BodyId bodyId,
            BodySeatId bodySeatId,
            DelegationAssignmentId delegationAssignmentId)
        {
            Id = bodyId;

            BodyMandateId = bodyMandateId;
            BodySeatId = bodySeatId;
            DelegationAssignmentId = delegationAssignmentId;
        }
    }
}
