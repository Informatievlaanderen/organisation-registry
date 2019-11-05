export class OrganisationOpeningHourListItem {
  constructor(
    public organisationOpeningHourId: string = '',
    public opens: string = '',
    public closes: string = '',
    public dayOfWeek: Number = 0,
    public validFrom: Date,
    public validTo: Date,
    public labelValue: string
  ) { }
}
