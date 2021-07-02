export class OrganisationRegulation {
    constructor(
      public organisationId: string,
      public regulationId: string,
      public regulationTypeName: string,
      public link: string,
      public date: string,
      public description: string,
      public validFrom: Date,
      public validTo: Date
    ) {

    }
}
