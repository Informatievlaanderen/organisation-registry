export class OrganisationFunction {
    constructor(
      public organisationId: string,
      public functionId: string,
      public functionName: string,
      public personId: string,
      public personName: string,
      public validFrom: Date,
      public validTo: Date
    ) {
    }
}
