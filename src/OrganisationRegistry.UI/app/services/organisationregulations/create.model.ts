import { UUID } from 'angular2-uuid';

export class CreateOrganisationRegulationRequest {
  public organisationRegulationId: string = '';
  public organisationId: string = '';
  public regulationTypeId: string = '';

  public link: string = '';
  public date: string = '';
  public description: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationRegulationId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
