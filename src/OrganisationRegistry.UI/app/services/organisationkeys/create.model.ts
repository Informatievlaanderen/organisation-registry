import { UUID } from 'angular2-uuid';

export class CreateOrganisationKeyRequest {
  public organisationKeyId: string = '';
  public organisationId: string = '';
  public keyTypeId: string = '';

  public keyValue: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationKeyId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
