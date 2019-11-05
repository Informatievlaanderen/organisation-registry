export class BodyBodyClassification {
    constructor(
      public bodyId: string,
      public bodyClassificationTypeId: string,
      public bodyClassificationTypeName: string,
      public bodyClassificationId: string,
      public bodyClassificationName: string,
      public validFrom: Date,
      public validTo: Date
    ) {
    }
}
