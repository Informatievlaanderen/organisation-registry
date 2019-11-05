export class LifecyclePhaseTypeListItem {
  constructor(
    public id: string = '',
    public name: string = '',
    public representsActivePhase: boolean = false,
    public isDefaultPhase: boolean = false
  ) { }
}
