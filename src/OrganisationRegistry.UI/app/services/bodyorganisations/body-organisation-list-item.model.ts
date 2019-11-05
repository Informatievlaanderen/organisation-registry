export class BodyOrganisationListItem {
  constructor(
    public bodyOrganisationId: string = '',
    public organisationId: string = '',
    public organisationName: string = '',
    public validFrom: Date,
    public validTo: Date
  ) { }
}
