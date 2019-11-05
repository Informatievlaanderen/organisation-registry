export class BodyContactListItem {
  constructor(
    public bodyContactId: string = '',
    public contactId: string = '',
    public contactTypeName: string = '',
    public validFrom: Date,
    public validTo: Date,
    public contactValue: string
  ) { }
}
