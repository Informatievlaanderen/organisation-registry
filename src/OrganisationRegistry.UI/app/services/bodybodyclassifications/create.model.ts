import { UUID } from 'angular2-uuid';

export class CreateBodyBodyClassificationRequest {
  public bodyBodyClassificationId: string = '';
  public bodyId: string = '';
  public bodyClassificationTypeId: string = '';

  public bodyClassificationId: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(bodyId) {
    this.bodyBodyClassificationId = UUID.UUID();
    this.bodyId = bodyId;
  }
}
