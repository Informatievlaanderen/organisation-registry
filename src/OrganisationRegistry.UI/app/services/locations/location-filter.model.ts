export class LocationFilter {
  constructor(
    public street: string = '',
    public zipCode: string = '',
    public city: string = '',
    public country: string = '',
    public nonCrabOnly: boolean = false
  ) { }
}
