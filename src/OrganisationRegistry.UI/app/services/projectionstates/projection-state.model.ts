import { ICrudItem } from 'core/crud';

export class ProjectionState implements ICrudItem<ProjectionState> {
  constructor(
    public id: string = '',
    public name: string = '',
    public eventNumber: number = 0
  ) { }
}
