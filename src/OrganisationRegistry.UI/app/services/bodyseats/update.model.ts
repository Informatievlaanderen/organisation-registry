export class UpdateBodySeatRequest {
  constructor(
    public bodySeatId: string,
    public bodyId: string,
    public name: string,
    public paidSeat: boolean,
    public entitledToVote: boolean = false,
    public validFrom: Date,
    public validTo: Date
  ) {
  }
}
