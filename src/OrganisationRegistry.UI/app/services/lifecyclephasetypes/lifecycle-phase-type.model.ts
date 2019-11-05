import { ICrudItem } from 'core/crud';

export class LifecyclePhaseType implements ICrudItem<LifecyclePhaseType> {
  constructor(
    public id: string = '',
    public name: string = '',
    public representsActivePhase: boolean = false,
    public isDefaultPhase: boolean = false
  ) { }
}
