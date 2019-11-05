import { ICrudItem } from 'core/crud';

export class Delegation implements ICrudItem<Delegation> {
  constructor(
    public id: string = '',
    public name: string = '',

    public bodyId: string = '',
    public bodyName: string = '',
    public bodyOrganisationName: string = '',
    public organisationName: string = '',
    public functionTypeName: string = '',
    public bodySeatId: string = '',
    public bodySeatNumber: string = '',
    public bodySeatName: string = '',

    public validFrom: Date = null,
    public validTo: Date = null,

    public isDelegated: boolean = false
  ) { }
}
