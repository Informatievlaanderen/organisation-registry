export class UpdateBodyLifecyclePhaseRequest {
  constructor(
    public bodyLifecyclePhaseId: string,
    public bodyId: string,
    public lifecyclePhaseTypeId: string,
    public validFrom: Date,
    public validTo: Date
  ) {
  }
}
