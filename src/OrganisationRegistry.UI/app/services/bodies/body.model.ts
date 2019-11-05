import { ICrudItem } from 'core/crud';

export class Body implements ICrudItem<Body>, ICreateBody {
  constructor(
    public id: string = '',
    public bodyNumber: string = '',
    public shortName: string = '',
    public name: string = '',
    public description: string = '',
    public validFrom: Date = null,
    public validTo: Date = null,
    public formalValidFrom: Date = null,
    public formalValidTo: Date = null,
    public organisationId: string = '',
    public lifecycleValid: boolean = false,
    public hasAllSeatsAssigned: boolean = false,
    public isMepCompliant: boolean = false
  ) {
  }
}

export interface ICreateBody {
    id: string;
    shortName: string;
    name: string;
    description: string;
    validFrom: Date;
    validTo: Date;
    formalValidFrom: Date;
    formalValidTo: Date;
    organisationId: string;
}
