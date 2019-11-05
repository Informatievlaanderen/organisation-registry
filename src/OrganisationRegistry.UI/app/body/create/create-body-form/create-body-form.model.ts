import { UUID } from 'angular2-uuid';
import { ICrudItem } from 'core/crud';

export class CreateBodyFormValues implements ICrudItem<CreateBodyFormValues> {
  constructor(
    public id: string = UUID.UUID(),
    public bodyNumber: string = '',
    public name: string = '',
    public shortName: string = '',
    public description: string = '',
    public validFrom: Date = null,
    public validTo: Date = null,
    public formalValidFrom: Date = null,
    public formalValidTo: Date = null,
    public organisationId: string = '',
  ) { }
}
