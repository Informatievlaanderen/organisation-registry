export class OrganisationFormalFrameworkListItem {
  constructor(
    public organisationFormalFrameworkId: string = '',
    public formalFrameworkId: string = '',
    public formalFrameworkName: string = '',
    public validFrom: Date,
    public validTo: Date
  ) { }
}
