import { ICrudItem } from 'core/crud';

export class OrganisationClassificationType implements ICrudItem<OrganisationClassificationType> {
  constructor(
    public id: string = '',
    public name: string = ''
  ) { }
}
