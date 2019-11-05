import { UUID } from 'angular2-uuid';

export class CreateBodyOrganisationRequest {
  public bodyOrganisationId: string = '';
  public bodyId: string = '';
  public organisationId: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(bodyId) {
    this.bodyOrganisationId = UUID.UUID();
    this.bodyId = bodyId;
  }
}
