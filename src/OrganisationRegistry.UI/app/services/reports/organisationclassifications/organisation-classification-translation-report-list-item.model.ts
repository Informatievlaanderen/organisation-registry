export class OrganisationClassificationTranslationReportListItem {
  constructor(
    public parentOrganisationId: string = '',
    public parentOrganisationName: string = '',
    public organisationId: string = '',
    public organisationName: string = '',
    public organisationShortName: string = '',
    public organisationNameFrench: string = '',
    public organisationNameEnglish: string = '',
    public organisationNameGerman: string = ''
  ) { }
}
