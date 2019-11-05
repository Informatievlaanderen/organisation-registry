export class OrganisationRelationListItem {
  constructor(
    public organisationRelationId: string = '',
    public relationId: string = '',
    public relationName: string = '',
    public validFrom: Date,
    public validTo: Date,
    public relatedOrganisationId: string = '',
    public relatedOrganisationName: string = ''
  ) { }
}
