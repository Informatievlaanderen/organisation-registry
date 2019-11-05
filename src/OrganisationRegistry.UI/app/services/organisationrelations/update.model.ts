export class UpdateOrganisationRelationRequest {
  constructor(
    public organisationRelationId: string,
    public organisationId: string,
    public relationId: string,
    public validFrom: Date,
    public validTo: Date,
  ) {
  }
}
