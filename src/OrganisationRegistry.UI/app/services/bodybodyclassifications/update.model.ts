export class UpdateBodyBodyClassificationRequest {
  constructor(
    public bodyBodyClassificationId: string,
    public bodyId: string,
    public bodyClassificationTypeId: string,
    public bodyClassificationId: string,
    public validFrom: Date,
    public validTo: Date
  ) {
  }
}
