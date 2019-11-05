export class OrganisationLocation {
  constructor(
      public organisationLocationId: string,
      public organisationId: string,
      public locationId: string,
      public locationName: string,
      public validFrom: Date,
      public validTo: Date
    ) {

    }
}
