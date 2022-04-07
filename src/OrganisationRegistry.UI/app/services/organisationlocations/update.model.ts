export class UpdateOrganisationLocationRequest {
  constructor(
    public organisationLocationId: string,
    public organisationId: string,
    public locationId: string,
    public locationValue: string,
    public locationTypeId: string,
    public locationTypeValue: string,
    public validFrom: Date,
    public validTo: Date,
    public source: string
  ) {
  }
}
