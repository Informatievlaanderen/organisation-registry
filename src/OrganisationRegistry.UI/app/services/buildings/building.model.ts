import { ICrudItem } from 'core/crud';

export class Building implements ICrudItem<Building> {
  constructor(
    public id: string = '',
    public name: string = '',
    public vimId: string = ''
  ) { }
}
