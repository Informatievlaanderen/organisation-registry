export class BodySeatListItem {
  constructor(
    public bodySeatId: string = '',
    public name: string = '',
    public seatTypeName: string = '',
    public validFrom: Date,
    public validTo: Date
  ) { }
}
