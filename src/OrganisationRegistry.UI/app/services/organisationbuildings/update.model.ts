export class UpdateOrganisationBuildingRequest {
  constructor(
    public organisationBuildingId: string,
    public organisationId: string,
    public buildingId: string,
    public buildingValue: string,
    public validFrom: Date,
    public validTo: Date
  ) {
  }
}
