import { UUID } from 'angular2-uuid';

export class CreateOrganisationRegulationRequest {
  public organisationRegulationId: string = '';
  public organisationId: string = '';
  public regulationThemeId: string = '';
  public regulationSubThemeId: string = '';

  public regulationName: string = '';
  public regulationUrl: string = '';
  public regulationDate: string = '';
  public description: string = '';
  public descriptionRendered: string = '';
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationRegulationId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
