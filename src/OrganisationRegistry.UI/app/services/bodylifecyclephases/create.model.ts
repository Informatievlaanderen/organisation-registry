import { UUID } from 'angular2-uuid';

export class CreateBodyLifecyclePhaseRequest {
  public bodyLifecyclePhaseId: string = '';
  public bodyId: string = '';
  public lifecyclePhaseTypeId: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(bodyId: string) {
    this.bodyId = bodyId;
    this.bodyLifecyclePhaseId = UUID.UUID();
  }
}
