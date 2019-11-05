export class OrganisationFilter {
  constructor(
    public name: string = '',
    public ovoNumber: string = '',
    public activeOnly: boolean = true,
    public formalFrameworkId: string = '',
    public organisationClassificationTypeId: string = '',
    public organisationClassificationId: string = '',
    public authorizedOnly: boolean = false
  ) { }

  withName(name: string) {
    let copy = this.copy();
    copy.name = name;
    return copy;
  }

  withActiveOnly(activeOnly: boolean) {
    let copy = this.copy();
    copy.activeOnly = activeOnly;
    return copy;
  }

  withAuthorizedOnly(authorizedOnly: boolean) {
    let copy = this.copy();
    copy.authorizedOnly = authorizedOnly;
    return copy;
  }

  private copy() {
    return new OrganisationFilter(
      this.name,
      this.ovoNumber,
      this.activeOnly,
      this.formalFrameworkId,
      this.organisationClassificationTypeId,
      this.organisationClassificationId,
      this.authorizedOnly
    );
  }
}
