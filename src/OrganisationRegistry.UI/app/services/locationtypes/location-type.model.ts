import { ICrudItem } from 'core/crud';

export class LocationType implements ICrudItem<LocationType> {
  constructor(
    public id: string = '',
    public name: string = ''
  ) { }
}
