export class OrganisationFormalFramework {
    constructor(
      public organisationId: string,
      public formalFrameworkId: string,
      public formalFrameworkName: string,
      public validFrom: Date,
      public validTo: Date
    ) {
    }
}
