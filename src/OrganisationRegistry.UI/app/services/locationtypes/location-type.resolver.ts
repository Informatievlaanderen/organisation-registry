import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';

import { LocationTypeService } from './location-type.service';
import { LocationTypeListItem } from './location-type-list-item.model';

@Injectable()
export class LocationTypeResolver implements Resolve<LocationTypeListItem[]> {

  constructor(
    private locationTypeService: LocationTypeService
  ) { }

  resolve(route: ActivatedRouteSnapshot) {
    return this.locationTypeService.getAllUserPermittedLocationTypes();
  }
}
