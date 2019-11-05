export class OrganisationListItem {
  constructor(
    public id: string = '',
    public ovoNumber: string = '',
    public name: string = '',
    public parentOrganisation: string = '',
    public parentOrganisationId: string = ''
  ) { }
}
