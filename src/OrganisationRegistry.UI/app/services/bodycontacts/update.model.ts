export class UpdateBodyContactRequest {
  constructor(
    public bodyContactId: string,
    public bodyId: string,
    public contactTypeId: string,
    public contactValue: string,
    public validFrom: Date,
    public validTo: Date
  ) {

  }
}
