export class OrganisationContact {
    constructor(
      public organisationId: string,
      public contactId: string,
      public contactTypeName: string,
      public contactValue: string,
      public validFrom: Date,
      public validTo: Date
    ) {

    }
}
