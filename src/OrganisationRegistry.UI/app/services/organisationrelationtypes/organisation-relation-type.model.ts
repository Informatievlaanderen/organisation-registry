import { ICrudItem } from 'core/crud';

export class OrganisationRelationType implements ICrudItem<OrganisationRelationType> {
  constructor(
    public id: string = '',
    public name: string = '',
    public inverseName: string = ''
  ) { }
}
