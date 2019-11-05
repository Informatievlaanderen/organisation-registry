import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';

import { SeatTypeService } from './seat-type.service';
import { SeatTypeListItem } from './seat-type-list-item.model';

@Injectable()
export class SeatTypeResolver implements Resolve<SeatTypeListItem[]> {

  constructor(
    private seatTypeService: SeatTypeService
  ) { }

  resolve(route: ActivatedRouteSnapshot) {
    return this.seatTypeService.getAllSeatTypes();
  }
}
