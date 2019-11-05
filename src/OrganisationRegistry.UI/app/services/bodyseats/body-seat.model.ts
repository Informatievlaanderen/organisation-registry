export class BodySeat {
    constructor(
      public bodyId: string,
      public name: string,
      public paidSeat: boolean,
      public validFrom: Date,
      public validTo: Date
    ) {
    }
}
