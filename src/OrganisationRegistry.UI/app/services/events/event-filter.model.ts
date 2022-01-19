export class EventFilter {
  constructor(
    public aggregateId: string = '',
    public eventNumber: string = '',
    public name: string = '',
    public firstName: string = '',
    public lastName: string = '',
    public data: string = '',
    public ip: string = ''
  ) { }
}
