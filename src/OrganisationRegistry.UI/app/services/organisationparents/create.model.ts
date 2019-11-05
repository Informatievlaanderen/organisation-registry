import { UUID } from 'angular2-uuid';

export class CreateOrganisationParentRequest {
  public organisationOrganisationParentId: string = '';
  public organisationId: string = '';
  public parentOrganisationId: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationOrganisationParentId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
