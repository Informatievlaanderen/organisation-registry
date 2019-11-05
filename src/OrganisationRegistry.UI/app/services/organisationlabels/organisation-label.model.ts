export class OrganisationLabel {
    constructor(
      public organisationId: string,
      public labelId: string,
      public labelTypeName: string,
      public labelValue: string,
      public validFrom: Date,
      public validTo: Date
    ) {

    }
}
