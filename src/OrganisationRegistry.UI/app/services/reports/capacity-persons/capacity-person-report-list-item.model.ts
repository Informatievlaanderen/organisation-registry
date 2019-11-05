export class CapacityPersonReportListItem {
  constructor(
    public parentOrganisationId: string = '',
    public parentOrganisationName: string = '',
    public organisationId: string = '',
    public organisationName: string = '',
    public organisationShortName: string = '',
    public personName: string = '',
    public personId: string = '',
    public functionTypeId: string = '',
    public functionTypeName: string = '',
    public email: string = '',
    public phone: string = '',
    public cellPhone: string = '',
    public location: string = ''
  ) { }
}
