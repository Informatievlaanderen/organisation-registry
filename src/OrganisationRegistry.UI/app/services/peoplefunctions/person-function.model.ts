export class PersonFunction {
    constructor(
      public organisationId: string,
      public organisationName: string,
      public functionId: string,
      public functionName: string,
      public personId: string,
      public validFrom: Date,
      public validTo: Date
    ) {
    }
}
