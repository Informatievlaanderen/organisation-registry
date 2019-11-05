export class OrganisationBuildingListItem {
  constructor(
    public organisationBuildingId: string = '',
    public buildingId: string = '',
    public buildingName: string = '',
    public validFrom: Date,
    public validTo: Date,
    public isActive: boolean
  ) { }
}
