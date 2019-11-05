export class BodyLifecyclePhaseListItem {
  constructor(
    public bodyLifecyclePhaseId: string = '',
    public lifecyclePhaseTypeId: string = '',
    public lifecyclePhaseTypeName: string = '',
    public validFrom: Date,
    public validTo: Date,
    public hasAdjacentGaps: boolean = false
  ) { }
}
