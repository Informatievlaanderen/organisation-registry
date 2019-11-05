export class OrganisationContactListItem {
  constructor(
    public organisationContactId: string = '',
    public contactId: string = '',
    public contactTypeName: string = '',
    public validFrom: Date,
    public validTo: Date,
    public contactValue: string
  ) { }
}
