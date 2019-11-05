import { ICrudItem } from 'core/crud';

export class KeyType implements ICrudItem<KeyType> {
  constructor(
    public id: string = '',
    public name: string = ''
  ) { }
}
