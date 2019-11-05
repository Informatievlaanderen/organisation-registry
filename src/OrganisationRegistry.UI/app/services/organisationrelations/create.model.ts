import { UUID } from 'angular2-uuid';

export class CreateOrganisationRelationRequest {
  public organisationRelationId: string = '';
  public organisationId: string = '';
  public relationId: string = '';
  public relatedOrganisationId: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationRelationId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
