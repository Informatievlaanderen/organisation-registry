import { UUID } from 'angular2-uuid';

export class CreateOrganisationOrganisationClassificationRequest {
  public organisationOrganisationClassificationId: string = '';
  public organisationId: string = '';
  public organisationClassificationTypeId: string = '';

  public organisationClassificationId: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationOrganisationClassificationId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
