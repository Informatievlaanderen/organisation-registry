export class DelegationListItem {
  constructor(
    public id: string = '',

    public organisationId: string = '',
    public organisationName: string = '',
    public bodyId: string = '',
    public bodyName: string = '',
    public bodySeatName: string = '',
    public isDelegated: boolean = false
  ) { }
}
