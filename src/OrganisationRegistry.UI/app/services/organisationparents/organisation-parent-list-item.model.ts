export class OrganisationParentListItem {
  constructor(
    public organisationOrganisationParentId: string = '',
    public parentOrganisationId: string = '',
    public parentOrganisationName: string = '',
    public validFrom: Date,
    public validTo: Date
  ) { }
}
