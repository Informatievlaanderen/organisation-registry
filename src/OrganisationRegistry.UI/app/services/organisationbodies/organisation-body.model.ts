export class OrganisationBody {
    constructor(
      public organisationId: string,
      public bodyId: string,
      public bodyName: string,
      public validFrom: Date,
      public validTo: Date
    ) {
    }
}
