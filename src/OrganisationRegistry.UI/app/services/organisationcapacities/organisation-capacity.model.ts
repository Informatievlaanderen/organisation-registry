export class OrganisationCapacity {
    constructor(
      public organisationId: string,
      public capacityId: string,
      public capacityName: string,
      public personId: string,
      public personName: string,
      public functionId: string,
      public functionName: string,
      public locationId: string,
      public locationName: string,
      public validFrom: Date,
      public validTo: Date
    ) {
    }
}
