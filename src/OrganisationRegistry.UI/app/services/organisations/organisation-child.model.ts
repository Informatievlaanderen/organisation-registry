import { ICrudItem } from 'core/crud';

export class OrganisationChild implements ICrudItem<OrganisationChild> {
  constructor(
    public id: string = '',
    public ovoNumber: string = '',
    public name: string = '',
  ) {
   }
}
