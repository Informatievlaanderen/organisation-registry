export class UpdateOrganisationContactRequest {
  constructor(
    public organisationContactId: string,
    public organisationId: string,
    public contactTypeId: string,
    public contactValue: string,
    public validFrom: Date,
    public validTo: Date
  ) {

  }
}
