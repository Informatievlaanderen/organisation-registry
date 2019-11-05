export class OrganisationRelation {
    constructor(
      public organisationId: string,
      public relationId: string,
      public relationName: string,
      public relatedOrganisationId: string,
      public relatedOrganisationName: string,
      public validFrom: Date,
      public validTo: Date
    ) {
    }
}
