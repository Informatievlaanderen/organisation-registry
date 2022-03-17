export class UpdateOrganisationRegulationRequest {
  constructor(
    public organisationRegulationId: string,
    public organisationId: string,
    public regulationThemeId: string,
    public name: string,
    public description: string,
    public uri: string,
    public workRulesUrl: string,
    public validFrom: Date,
    public validTo: Date
  ) {

  }
}
