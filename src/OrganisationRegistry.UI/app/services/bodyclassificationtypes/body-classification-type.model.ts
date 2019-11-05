import { ICrudItem } from 'core/crud';

export class BodyClassificationType implements ICrudItem<BodyClassificationType> {
  constructor(
    public id: string = '',
    public name: string = ''
  ) { }
}
