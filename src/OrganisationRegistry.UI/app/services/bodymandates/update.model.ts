export class UpdateBodyMandateRequest {
  constructor(
    public bodyMandateId: string,
    public bodyId: string,
    public name: string,
    public validFrom: Date,
    public validTo: Date
  ) {
  }
}
