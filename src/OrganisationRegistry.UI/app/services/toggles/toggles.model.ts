export class Toggles  {
  constructor(
    public applicationAvailable: boolean = false,
    public logToElasticSearch: boolean = false,
    public apiAvailable: boolean = false,
    public elasticSearchProjectionsAvailable: boolean = false,
    public vlaanderenBeNotifierAvailable: boolean = false,
    public sendVlaanderenBeNotifierMails: boolean = false,
    public enableReporting: boolean = false,
    public enableVademecumParticipationReporting: boolean = false,
    public enableOrganisationRelations: boolean = false,
    public enableFormalFrameworkBodiesReporting: boolean = false,
    public enableOrganisationOpeningHours: boolean = false
  ) { }
}
