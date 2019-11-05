import { UUID } from 'angular2-uuid';

import { ICrudItem } from 'core/crud';

export class Location implements ICrudItem<Location>{
  public crabLocationId: string = '';
  public formattedAddress: string = '';
  public street: string = '';
  public zipCode: string = '';
  public city: string = '';
  public country: string = '';
  public id: string = '';

  constructor(
  ) {
    this.id = UUID.UUID();
  }

  public get name() {
    return `${this.street}, ${this.zipCode} ${this.city}`;
  }

  public withValues(location: Location): Location {
    let newValue = new Location();
    newValue.id = location.id;
    newValue.crabLocationId = location.crabLocationId;
    newValue.formattedAddress = location.formattedAddress;
    newValue.street = location.street;
    newValue.zipCode = location.zipCode;
    newValue.city = location.city;
    newValue.country = location.country;

    return newValue;
  }
}
