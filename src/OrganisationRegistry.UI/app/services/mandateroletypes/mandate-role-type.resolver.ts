import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';

import { MandateRoleTypeService } from './mandate-role-type.service';
import { MandateRoleTypeListItem } from './mandate-role-type-list-item.model';

@Injectable()
export class MandateRoleTypeResolver implements Resolve<MandateRoleTypeListItem[]> {

  constructor(
    private mandateRoleTypeService: MandateRoleTypeService
  ) { }

  resolve(route: ActivatedRouteSnapshot) {
    return this.mandateRoleTypeService.getAllMandateRoleTypes();
  }
}
