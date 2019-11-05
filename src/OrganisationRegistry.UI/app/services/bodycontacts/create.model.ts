import { UUID } from 'angular2-uuid';

export class CreateBodyContactRequest {
  public bodyContactId: string = '';
  public bodyId: string = '';
  public contactTypeId: string = '';

  public contactValue: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(bodyId) {
    this.bodyContactId = UUID.UUID();
    this.bodyId = bodyId;
  }
}
