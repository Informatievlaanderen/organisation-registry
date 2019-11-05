export class UpdateOrganisationCapacityRequest {
  constructor(
    public organisationCapacityId: string,
    public organisationId: string,
    public capacityId: string,
    public personId: string,
    public functionId: string,
    public validFrom: Date,
    public validTo: Date,
    public contacts: any
  ) {
  }
}
