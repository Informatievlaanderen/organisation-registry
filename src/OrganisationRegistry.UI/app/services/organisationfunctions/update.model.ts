export class UpdateOrganisationFunctionRequest {
  constructor(
    public organisationFunctionId: string,
    public organisationId: string,
    public functionId: string,
    public personId: string,
    public validFrom: Date,
    public validTo: Date,
    public contacts: any
  ) {
  }
}
