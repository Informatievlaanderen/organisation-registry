export class OrganisationKey {
    constructor(
      public organisationId: string,
      public keyTypeId: string,
      public keyTypeName: string,
      public keyValue: string,
      public validFrom: Date,
      public validTo: Date
    ) {

    }
}
