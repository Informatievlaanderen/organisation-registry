import { UUID } from 'angular2-uuid';

export class CreateOrganisationLabelRequest {
  public organisationLabelId: string = '';
  public organisationId: string = '';
  public labelTypeId: string = '';

  public labelValue: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationLabelId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
