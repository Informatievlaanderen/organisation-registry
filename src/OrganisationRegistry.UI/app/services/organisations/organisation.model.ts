import { ICrudItem } from 'core/crud';
import {UUID} from "angular2-uuid";

export class Organisation implements ICrudItem<Organisation>, ICreateOrganisation {
  constructor(
    public id: string = '',
    public ovoNumber: string = '',
    public kboNumber: string = '',
    public shortName: string = '',
    public name: string = '',
    public parentOrganisation: string = '',
    public parentOrganisationId: string = '',
    public description: string = '',
    public validFrom: Date = null,
    public validTo: Date = null,
    public purposeIds: Array<string> = [],
    public purposes: Array<string> = [],
    public showOnVlaamseOverheidSites: boolean = false
  ) {
  }
}

class KboName {
  public value: string = '';
}

export class KboOrganisation {
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

export interface ICreateOrganisation {
  id: string;
  shortName: string;
  name: string;
  parentOrganisationId: string;
  description: string;
  purposeIds: Array<string>;
  purposes: Array<string>;
  showOnVlaamseOverheidSites: boolean;
  validFrom: Date;
  validTo: Date;
}
