export class OrganisationFilter {
  constructor(
    public name: string = '',
    public ovoNumber: string = '',
    public code: string = '',
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

  withOvoNumber(ovoNumber: string) {
    let copy = this.copy();
    copy.ovoNumber = ovoNumber;
    return copy;
  }

  withCode(code: string) {
    let copy = this.copy();
    copy.code = code;
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
      this.code,
      this.activeOnly,
      this.formalFrameworkId,
      this.organisationClassificationTypeId,
      this.organisationClassificationId,
      this.authorizedOnly
    );
  }
}
