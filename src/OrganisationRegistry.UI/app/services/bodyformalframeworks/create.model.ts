import { UUID } from 'angular2-uuid';

export class CreateBodyFormalFrameworkRequest {
  public bodyFormalFrameworkId: string = '';
  public bodyId: string = '';
  public formalFrameworkId: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(bodyId: string) {
    this.bodyId = bodyId;
    this.bodyFormalFrameworkId = UUID.UUID();
  }
}
