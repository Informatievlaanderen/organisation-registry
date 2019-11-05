import { ICrudItem } from 'core/crud';

export class MandateRoleType implements ICrudItem<MandateRoleType> {
  constructor(
    public id: string = '',
    public name: string = ''
  ) { }
}
