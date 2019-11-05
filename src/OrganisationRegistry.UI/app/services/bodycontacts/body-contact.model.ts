export class BodyContact {
  constructor(
    public bodyId: string,
    public contactId: string,
    public contactTypeName: string,
    public contactValue: string,
    public validFrom: Date,
    public validTo: Date
  ) {

  }
}
