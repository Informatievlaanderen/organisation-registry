import { ICrudItem } from 'core/crud';

export class LabelType implements ICrudItem<LabelType> {
  constructor(
    public id: string = '',
    public name: string = ''
  ) { }
}
