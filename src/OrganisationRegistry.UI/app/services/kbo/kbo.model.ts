import { UUID } from 'angular2-uuid';

class KboName {
  public value: string = '';
}

export class Kbo {
  constructor(
    public id: string = UUID.UUID(),
    public formalName: KboName = null,
    public shortName: KboName = null,
    public kboNumber: string = '',
    public description: string = '',
    public validFrom: Date = null,
    public validTo: Date = null,
  ) { }
}
