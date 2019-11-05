export class UpdateOrganisationOpeningHourRequest {
  constructor(
    public organisationOpeningHourId: string,
    public organisationId: string,
    public opens: string,
    public closes: string,
    public dayOfWeek: Number,
    public validFrom: Date,
    public validTo: Date
  ) {

  }
}
