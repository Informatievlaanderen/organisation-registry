export class UpdateOrganisationLabelRequest {
  constructor(
    public organisationLabelId: string,
    public organisationId: string,
    public labelTypeId: string,
    public labelValue: string,
    public validFrom: Date,
    public validTo: Date
  ) {

  }
}
