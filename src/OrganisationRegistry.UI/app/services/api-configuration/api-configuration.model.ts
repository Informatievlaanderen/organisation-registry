export class ApiConfiguration  {
  constructor(
    public api: any,
    public applicationInsights: any,
    public configuration: any,
    public elasticSearch: any,
    public infrastructure: any,
    public logging: any,
    public serilog: any,
    public vlaanderenBeNotifier: any,
    public toggles: any,
    public sqlServer: any,
  ) { }
}
