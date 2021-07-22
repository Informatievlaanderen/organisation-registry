export class Toggles  {
  constructor(
    public applicationAvailable: boolean = false,
    public apiAvailable: boolean = false,
    public elasticSearchProjectionsAvailable: boolean = false,
    public vlaanderenBeNotifierAvailable: boolean = false,
    public sendVlaanderenBeNotifierMails: boolean = false,
  ) { }
}
