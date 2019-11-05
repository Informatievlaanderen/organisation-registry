export class DelegationAssignmentListItem {
  constructor(
    public id: string = '',

    public personId: string = '',
    public personName: string = '',
    public validFrom: Date = null,
    public validTo: Date = null
  ) { }
}
