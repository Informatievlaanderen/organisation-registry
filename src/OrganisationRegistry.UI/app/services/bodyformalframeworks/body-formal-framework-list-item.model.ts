export class BodyFormalFrameworkListItem {
  constructor(
    public bodyFormalFrameworkId: string = '',
    public formalFrameworkId: string = '',
    public formalFrameworkName: string = '',
    public validFrom: Date,
    public validTo: Date
  ) { }
}
