export class BodyMandateListItem {
  constructor(
    public bodyMandateId: string = '',
    public name: string = '',
    public mandateRoleTypeName: string = '',
    public validFrom: Date,
    public validTo: Date
  ) { }
}
