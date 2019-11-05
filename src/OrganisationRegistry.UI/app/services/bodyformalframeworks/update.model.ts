export class UpdateBodyFormalFrameworkRequest {
  constructor(
    public bodyFormalFrameworkId: string,
    public bodyId: string,
    public formalFrameworkId: string,
    public validFrom: Date,
    public validTo: Date
  ) {
  }
}
