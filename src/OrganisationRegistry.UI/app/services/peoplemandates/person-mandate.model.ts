export class PersonMandate {
  constructor(
    public bodyId: string,
    public bodyName: string,
    public bodySeatId: string,
    public bodySeatName: string,
    public validFrom: Date,
    public validTo: Date,
    public personId: string) {
    }
}
