export class PersonMandateListItem {
  constructor(
    public bodyMandateId: string = '',
    public bodyId: string = '',
    public bodyName: string = '',
    public bodyOrganisationId: string = '',
    public bodyOrganisationName: string = '',
    public bodySeatId: string = '',
    public bodySeatName: string = '',
    public bodySeatNumber: string = '',
    public paidSeat: boolean = false,
    public validFrom: Date,
    public validTo: Date,
    public personId: string = ''
  ) { }
}
