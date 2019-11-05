export class BuildingOrganisationReportListItem {
  constructor(
    public organisationId: string = '',
    public organisationName: string = '',
    public organisationShortName: string = '',
    public organisationOvoNumber: string = '',
    public dataVlaanderenOrganisationUri: string = '',
    public legalForm: string = '',
    public policyDomain: string = '',
    public responsibleMinister: string = '',
  ) { }
}
