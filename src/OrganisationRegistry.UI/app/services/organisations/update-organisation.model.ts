export class UpdateOrganisationRequest {
  constructor(
    public name: string = '',
    public shortName: string = '',
    public description: string = '',
    public validFrom: Date = null,
    public validTo: Date = null,
    public operationalValidFrom: Date = null,
    public operationalValidTo: Date = null,
  ) { }
}
