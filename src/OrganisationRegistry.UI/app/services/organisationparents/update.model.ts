export class UpdateOrganisationParentRequest {
  constructor(
    public organisationOrganisationParentId: string,
    public organisationId: string,
    public parentOrganisationId: string,
    public validFrom: Date,
    public validTo: Date
  ) {
  }
}
