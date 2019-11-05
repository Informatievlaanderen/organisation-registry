export class OrganisationBodyListItem {
  constructor(
    public organisationBodyId: string = '',
    public bodyId: string = '',
    public bodyName: string = '',
    public validFrom: Date,
    public validTo: Date
  ) { }
}
