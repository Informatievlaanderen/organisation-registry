import { UUID } from 'angular2-uuid';

export class CreateBodySeatRequest {
  public bodySeatId: string = '';
  public bodyId: string = '';
  public name: string = '';
  public seatTypeId: string = '';
  public paidSeat: boolean = false;
  public entitledToVote: boolean = false;
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(bodyId: string) {
    this.bodyId = bodyId;
    this.bodySeatId = UUID.UUID();
  }
}
