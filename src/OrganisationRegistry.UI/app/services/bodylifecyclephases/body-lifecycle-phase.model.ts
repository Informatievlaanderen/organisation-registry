export class BodyLifecyclePhase {
    constructor(
      public bodyId: string,
      public lifecyclePhaseTypeId: string,
      public lifecyclePhaseTypeName: string,
      public validFrom: Date,
      public validTo: Date
    ) {
    }
}
