import { UUID } from 'angular2-uuid';

export class Kbo {
  constructor(
    public id: string = UUID.UUID(),
    public name: string = '',
    public shortName: string = '',
    public kboNumber: string = '',
    public description: string = '',
    public validFrom: Date = null,
    public validTo: Date = null,
  ) { }
}
