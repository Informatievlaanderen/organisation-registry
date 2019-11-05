import { ICrudItem } from 'core/crud';

export class Purpose implements ICrudItem<Purpose> {
  constructor(
    public id: string = '',
    public name: string = ''
  ) { }
}
