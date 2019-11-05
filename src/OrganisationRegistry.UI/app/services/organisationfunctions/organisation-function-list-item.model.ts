export class OrganisationFunctionListItem {
  constructor(
    public organisationFunctionId: string = '',
    public functionId: string = '',
    public functionName: string = '',
    public validFrom: Date,
    public validTo: Date,
    public personId: string = '',
    public personName: string = ''
  ) { }
}
