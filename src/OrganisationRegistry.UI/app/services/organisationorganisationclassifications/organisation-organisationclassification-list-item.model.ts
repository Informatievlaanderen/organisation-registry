export class OrganisationOrganisationClassificationListItem {
  constructor(
    public organisationOrganisationClassificationId: string = '',
    public organisationClassificationTypeId: string = '',
    public organisationClassificationTypeName: string = '',
    public validFrom: Date,
    public validTo: Date,
    public organisationClassificationId: string = '',
    public organisationClassificationName: string = '',
    public isEditable: boolean = false,
  ) { }
}
