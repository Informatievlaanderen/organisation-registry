export class OrganisationCapacityListItem {
  constructor(
    public organisationCapacityId: string = '',
    public capacityId: string = '',
    public capacityName: string = '',
    public personId: string = '',
    public personName: string = '',
    public functionId: string = '',
    public functionName: string = '',
    public validFrom: Date,
    public validTo: Date
  ) { }
}
