export class BodyOrganisation {
    constructor(
      public bodyId: string,
      public organisationId: string,
      public organisationName: string,
      public validFrom: Date,
      public validTo: Date
    ) {
    }
}
