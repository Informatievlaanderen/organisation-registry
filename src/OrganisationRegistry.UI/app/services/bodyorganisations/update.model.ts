export class UpdateBodyOrganisationRequest {
  constructor(
    public bodyOrganisationId: string,
    public bodyId: string,
    public organisationId: string,
    public validFrom: Date,
    public validTo: Date
  ) {
  }
}
