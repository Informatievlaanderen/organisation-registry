export class UpdateOrganisationRegulationRequest {
  constructor(
    public organisationRegulationId: string,
    public organisationId: string,
    public regulationThemeId: string,
    public link: string,
    public validFrom: Date,
    public validTo: Date
  ) {

  }
}
