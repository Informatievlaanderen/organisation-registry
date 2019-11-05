import { UUID } from 'angular2-uuid';

import { BodyMandateType } from './body-mandate-type.model';

export class CreateBodyMandateRequest {
  public bodyMandateId: string = '';
  public bodyId: string = '';
  public bodySeatId: string = '';
  public bodyMandateType: BodyMandateType = null;
  public delegatorId: string = '';
  public delegatedId: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;
  public contacts = {};

  constructor(bodyId: string, bodyMandateType: BodyMandateType) {
    this.bodyId = bodyId;
    this.bodyMandateType = bodyMandateType;
    this.bodyMandateId = UUID.UUID();
  }
}
