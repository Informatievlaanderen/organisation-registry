import { UUID } from 'angular2-uuid';
import { ICrudItem } from 'core/crud';

export class CreateOrganisationFormValues implements ICrudItem<CreateOrganisationFormValues> {
  constructor(
    public id: string = UUID.UUID(),
    public name: string = '',
    public shortName: string = '',
    public article: string = '',
    public parentOrganisationId: string = '',
    public description: string = '',
    public purposeIds: Array<string> = [],
    public purposes: Array<string> = [],
    public showOnVlaamseOverheidSites: boolean = false,
    public validFrom: Date = null,
    public validTo: Date = null,
    public operationalValidFrom: Date = null,
    public operationalValidTo: Date = null,
  ) { }
}
