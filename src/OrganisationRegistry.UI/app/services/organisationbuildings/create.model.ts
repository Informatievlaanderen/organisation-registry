import { UUID } from 'angular2-uuid';

export class CreateOrganisationBuildingRequest {
  public organisationBuildingId: string = '';
  public organisationId: string = '';
  public buildingId: string = '';

  public isMainBuilding: Boolean = false;
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationBuildingId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
