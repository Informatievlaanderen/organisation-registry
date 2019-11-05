import { UUID } from 'angular2-uuid';
import { ICrudItem } from 'core/crud';

export class CreateOrganisationFormValues implements ICrudItem<CreateOrganisationFormValues> {
  constructor(
    public id: string = UUID.UUID(),
    public kboNumber: string = '',
    public name: string = '',
    public shortName: string = '',
  ) { }
}
