export class OrganisationParent {
  constructor(
    public parentOrganisationId: string = '',
    public parentOrganisationName: string = ''
  ) {
  }
}

export class OrganisationDocument {
  constructor(
    public id: string = '',
    public name: string = '',
    public ovoNumber: string = '',
    public shortName: string = '',
    public parents: OrganisationParent[] = []
  ) { }
}