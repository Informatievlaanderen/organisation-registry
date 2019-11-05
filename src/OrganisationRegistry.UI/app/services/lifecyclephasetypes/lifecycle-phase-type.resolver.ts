import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';

import { LifecyclePhaseTypeService } from './lifecycle-phase-type.service';
import { LifecyclePhaseTypeListItem } from './lifecycle-phase-type-list-item.model';

@Injectable()
export class LifecyclePhaseTypeResolver implements Resolve<LifecyclePhaseTypeListItem[]> {
  constructor(
    private lifecyclePhaseTypeService: LifecyclePhaseTypeService
  ) { }

  resolve(route: ActivatedRouteSnapshot) {
    return this.lifecyclePhaseTypeService.getAllLifecyclePhaseTypes();
  }
}
