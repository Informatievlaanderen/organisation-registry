export class OrganisationRegulationListItem {
  constructor(
    public organisationRegulationId: string = '',
    public regulationId: string = '',
    public regulationThemeName: string = '',
    public regulationSubThemeName: string = '',
    public name: string = '',
    public validFrom: Date,
    public validTo: Date,
    public link: string,
    public workRules: string,
    public isEditable: boolean,
  ) { }
}
