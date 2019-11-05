import { ICrudItem } from 'core/crud';

export class FormalFrameworkCategory implements ICrudItem<FormalFrameworkCategory> {
  constructor(
    public id: string = '',
    public name: string = ''
  ) { }
}
