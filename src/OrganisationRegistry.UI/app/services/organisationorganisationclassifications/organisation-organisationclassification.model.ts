export class OrganisationOrganisationClassification {
    constructor(
      public organisationId: string,
      public organisationClassificationTypeId: string,
      public organisationClassificationTypeName: string,
      public organisationClassificationId: string,
      public organisationClassificationName: string,
      public validFrom: Date,
      public validTo: Date
    ) {

    }
}
