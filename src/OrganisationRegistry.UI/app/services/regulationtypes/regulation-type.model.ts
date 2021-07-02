import { ICrudItem } from 'core/crud';

export class RegulationType implements ICrudItem<RegulationType> {
  constructor(
    public id: string = '',
    public name: string = ''
  ) { }
}
