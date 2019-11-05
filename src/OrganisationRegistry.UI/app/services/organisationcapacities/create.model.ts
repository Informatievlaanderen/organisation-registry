import { UUID } from 'angular2-uuid';

export class CreateOrganisationCapacityRequest {
  public organisationCapacityId: string = '';
  public organisationId: string = '';
  public capacityId: string = '';
  public personId: string = '';
  public functionId: string = '';
  public locationId: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;
  public contacts = {};

  constructor(organisationId) {
    this.organisationCapacityId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
