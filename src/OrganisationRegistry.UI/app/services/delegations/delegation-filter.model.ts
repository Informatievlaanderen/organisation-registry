export class DelegationFilter {
  constructor(
    public bodyName: string = '',
    public bodyOrganisationName: string = '',
    public organisationName: string = '',
    public functionTypeName: string = '',
    public bodySeatName: string = '',
    public bodySeatNumber: string = '',
    public activeMandatesOnly: boolean = true,
    public emptyDelegationsOnly: boolean = false
  ) { }
}
