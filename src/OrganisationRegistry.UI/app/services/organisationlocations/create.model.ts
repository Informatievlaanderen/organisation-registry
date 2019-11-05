import { UUID } from 'angular2-uuid';

export class CreateOrganisationLocationRequest {
  public organisationLocationId: string = '';
  public organisationId: string = '';
  public locationId: string = '';
  public locationTypeId: string = '';

  public isMainLocation: Boolean = false;
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationLocationId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
