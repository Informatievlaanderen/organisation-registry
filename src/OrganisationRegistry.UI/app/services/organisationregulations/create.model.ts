import { UUID } from 'angular2-uuid';

export class CreateOrganisationRegulationRequest {
  public organisationRegulationId: string = '';
  public organisationId: string = '';
  public regulationThemeId: string = '';
  public regulationSubThemeId: string = '';

  public name: string = '';
  public url: string = '';
  public date: string = '';
  public description: string = '';
  public descriptionRendered: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationRegulationId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
