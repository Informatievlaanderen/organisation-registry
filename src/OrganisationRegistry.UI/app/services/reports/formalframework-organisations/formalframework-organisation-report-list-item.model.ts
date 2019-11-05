export class FormalFrameworkOrganisationReportListItem {
  constructor(
    public organisationId: string = '',
    public organisationName: string = '',
    public organisationShortName: string = '',
    public organisationOvoNumber: string = '',
    public dataVlaanderenOrganisationUri: string = '',
    public legalForm: string = '',
    public policyDomain: string = '',
    public responsibleMinister: string = '',

    public INR: string = '',
    public KBO: string = '',
    public Orafin: string = '',
    public Vlimpers: string = '',
    public VlimpersKort: string = '',

    public Bestuursniveau: string = '',
    public Categorie: string = '',
    public Entiteitsvorm: string = '',
    public ESRKlasseToezichthoudendeOverheid: string = '',
    public ESRSector: string = '',
    public ESRToezichthoudendeOverheid: string = '',
    public UitvoerendNiveau: string = '',

    public ValidFrom: Date = null,
    public ValidTo: Date = null,
  ) { }
}
