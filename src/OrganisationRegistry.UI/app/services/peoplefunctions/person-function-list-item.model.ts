export class PersonFunctionListItem {
  constructor(
    public organisationFunctionId: string = '',
    public organisationName: string = '',
    public functionId: string = '',
    public functionName: string = '',
    public validFrom: Date,
    public validTo: Date,
    public personId: string = ''
  ) { }
}
