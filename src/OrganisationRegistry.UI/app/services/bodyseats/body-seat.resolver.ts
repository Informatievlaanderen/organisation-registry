import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';

import { BodySeatService } from './body-seat.service';
import { BodySeatListItem } from './body-seat-list-item.model';

@Injectable()
export class BodySeatResolver implements Resolve<BodySeatListItem[]> {

  constructor(
    private bodySeatService: BodySeatService
  ) { }

  resolve(route: ActivatedRouteSnapshot) {
    return this.bodySeatService.getAllBodySeats(route.parent.parent.params['id']);
  }
}
