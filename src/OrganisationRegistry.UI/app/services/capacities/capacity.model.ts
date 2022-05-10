import { ICrudItem } from 'core/crud';

export class Capacity implements ICrudItem<Capacity> {
  constructor(
    public id: string = '',
    public name: string = '',
    public isRemoved: boolean = false
  ) { }
}
