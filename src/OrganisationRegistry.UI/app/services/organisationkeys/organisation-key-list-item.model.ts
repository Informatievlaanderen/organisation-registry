export class OrganisationKeyListItem {
  constructor(
    public organisationKeyId: string = '',
    public keyTypeId: string = '',
    public keyTypeName: string = '',
    public validFrom: Date,
    public validTo: Date,
    public keyValue: string
  ) { }
}
