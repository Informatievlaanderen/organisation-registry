export class UpdateDelegationAssignmentRequest {
  constructor(
    public delegationAssignmentId: string,
    public bodyMandateId: string,
    public personId: string,
    public validFrom: Date,
    public validTo: Date,
    public contacts: any
  ) {
  }
}
