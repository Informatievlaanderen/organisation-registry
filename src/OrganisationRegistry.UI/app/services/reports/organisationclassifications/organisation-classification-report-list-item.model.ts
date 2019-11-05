export class OrganisationClassificationReportListItem {
  constructor(
    public id: string = '',
    public name: string = '',
    public order: number = 1,
    public active: boolean = true,
    public organisationClassificationTypeId: string = '',
    public organisationClassificationTypeName: string = ''
  ) { }
}
