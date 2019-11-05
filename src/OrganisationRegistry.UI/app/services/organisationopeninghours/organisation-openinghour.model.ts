export class OrganisationOpeningHour {
  constructor(
    public organisationId: string,
    public opens: string,
    public closes: string,
    public dayOfWeek: Number,
    public labelValue: string,
    public validFrom: Date,
    public validTo: Date
  ) {

  }
}
