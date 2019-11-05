export class UpdateOrganisationKeyRequest {
  constructor(
    public organisationKeyId: string,
    public organisationId: string,
    public keyTypeId: string,
    public keyValue: string,
    public validFrom: Date,
    public validTo: Date
  ) {

  }
}
