export class UpdateOrganisationOrganisationClassificationRequest {
  constructor(
    public organisationOrganisationClassificationId: string,
    public organisationId: string,
    public organisationClassificationTypeId: string,
    public organisationClassificationId: string,
    public validFrom: Date,
    public validTo: Date
  ) {

  }
}
