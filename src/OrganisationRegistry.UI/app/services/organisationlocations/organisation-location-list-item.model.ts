export class OrganisationLocationListItem {
  constructor(
    public organisationLocationId: string = '',
    public locationId: string = '',
    public locationName: string = '',
    public validFrom: Date,
    public validTo: Date,
    public isActive: boolean,
    public isKboLocation: boolean,
    public isEditable: boolean,
  ) { }
}
