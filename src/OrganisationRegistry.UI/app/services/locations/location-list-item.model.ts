export class LocationListItem {
  constructor(
    public id: string = '',
    public crabLocationId: string = '',
    public formattedAddress: string = '',
    public street: string = '',
    public zipCode: string = '',
    public city: string = '',
    public country: string = '',
  ) { }
}
