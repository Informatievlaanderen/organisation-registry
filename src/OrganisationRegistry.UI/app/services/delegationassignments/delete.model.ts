export class DeleteDelegationAssignmentRequest {
  constructor(
    public delegationAssignmentId: string,
    public bodyMandateId: string,
    public bodyId: string,
    public bodySeatId: string
  ) {
  }
}
