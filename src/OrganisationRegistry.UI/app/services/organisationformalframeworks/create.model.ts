import { UUID } from 'angular2-uuid';

export class CreateOrganisationFormalFrameworkRequest {
  public organisationFormalFrameworkId: string = '';
  public organisationId: string = '';
  public formalFrameworkId: string = '';
  public parentOrganisationId: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationFormalFrameworkId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
