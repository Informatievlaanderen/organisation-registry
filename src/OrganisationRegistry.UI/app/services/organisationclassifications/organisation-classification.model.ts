import { ICrudItem } from 'core/crud';

export class OrganisationClassification implements ICrudItem<OrganisationClassification> {
  constructor(
    public id: string = '',
    public name: string = '',
    public order: number = 1,
    public externalKey: string = '',
    public active: boolean = true,
    public organisationClassificationTypeId: string = '',
    public organisationClassificationTypeName: string = ''
  ) { }
}
