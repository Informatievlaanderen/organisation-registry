export class OrganisationParent {
    constructor(
      public organisationId: string,
      public parentOrganisationId: string,
      public parentOrganisationName: string,
      public validFrom: Date,
      public validTo: Date
    ) {
    }
}
