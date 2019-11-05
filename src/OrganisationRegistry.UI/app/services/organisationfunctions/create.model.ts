import { UUID } from 'angular2-uuid';

export class CreateOrganisationFunctionRequest {
  public organisationFunctionId: string = '';
  public organisationId: string = '';
  public functionId: string = '';
  public personId: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;
  public contacts = {};

  constructor(organisationId) {
    this.organisationFunctionId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
