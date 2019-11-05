import { ICrudItem } from 'core/crud';

export class DelegationAssignment implements ICrudItem<DelegationAssignment> {
  constructor(
    public id: string = '',
    public name: string = '',

    public personId: string = '',
    public personName: string = '',

    public validFrom: Date = null,
    public validTo: Date = null,
  ) { }
}
