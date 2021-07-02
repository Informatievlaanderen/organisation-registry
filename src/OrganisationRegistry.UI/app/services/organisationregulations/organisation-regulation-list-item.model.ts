export class OrganisationRegulationListItem {
  constructor(
    public organisationRegulationId: string = '',
    public regulationId: string = '',
    public regulationTypeName: string = '',
    public validFrom: Date,
    public validTo: Date,
    public link: string
  ) { }
}
