import { UUID } from 'angular2-uuid';

export class CreateDelegationAssignmentRequest {
  public delegationAssignmentId: string = '';
  public bodyId: string = '';
  public bodySeatId: string = '';
  public bodyMandateId: string = '';
  public personId: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;
  public contacts = {};

  constructor(bodyMandateId) {
    this.delegationAssignmentId = UUID.UUID();
    this.bodyMandateId = bodyMandateId;
  }
}
