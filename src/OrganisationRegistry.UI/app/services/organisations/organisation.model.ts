import { ICrudItem } from 'core/crud';

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

export class KboOrganisation implements ICrudItem<ICreateOrganisation> {
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
