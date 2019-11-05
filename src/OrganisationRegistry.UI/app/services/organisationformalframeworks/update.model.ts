export class UpdateOrganisationFormalFrameworkRequest {
  constructor(
    public organisationFormalFrameworkId: string,
    public organisationId: string,
    public formalFrameworkId: string,
    public validFrom: Date,
    public validTo: Date
  ) {
  }
}
