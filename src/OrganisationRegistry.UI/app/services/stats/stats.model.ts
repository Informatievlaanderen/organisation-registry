export class Stats  {
  constructor(
    public eventCount: number = -1,
    public organisationCount: number = -1,
    public requestCountPerDay: number = -1,
    public pageViewCountPerDay: number = -1,
    public exceptionCountPerDay: number = -1,
    public eventCountPerDay: number = -1
  ) { }
}
