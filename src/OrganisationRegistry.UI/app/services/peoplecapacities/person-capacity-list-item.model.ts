export class PersonCapacityListItem {
  constructor(
    public organisationCapacityId: string = '',
    public organisationName: string = '',
    public capacityId: string = '',
    public capacityName: string = '',
    public personId: string = '',
    public functionId: string = '',
    public functionName: string = '',
    public validFrom: Date,
    public validTo: Date
  ) { }
}
