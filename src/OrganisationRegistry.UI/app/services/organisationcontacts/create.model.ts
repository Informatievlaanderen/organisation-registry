import { UUID } from 'angular2-uuid';

export class CreateOrganisationContactRequest {
  public organisationContactId: string = '';
  public organisationId: string = '';
  public contactTypeId: string = '';

  public contactValue: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationContactId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
