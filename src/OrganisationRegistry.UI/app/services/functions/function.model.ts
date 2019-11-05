import { ICrudItem } from 'core/crud';

export class Function implements ICrudItem<Function> {
  constructor(
    public id: string = '',
    public name: string = ''
  ) { }
}
