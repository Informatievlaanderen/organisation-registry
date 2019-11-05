export class OrganisationClassificationReportFilter {
  constructor(
    public name: string = '',
    public active: boolean = true,
    public organisationClassificationTypeName: string = '',
    public organisationClassificationTypeId: string = ''
  ) { }
}
