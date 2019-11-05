export class Exception  {
  constructor(
    public timestamp: Date = null,
    public type: string = '',
    public method: string = '',
    public request: string = ''
  ) { }
}
