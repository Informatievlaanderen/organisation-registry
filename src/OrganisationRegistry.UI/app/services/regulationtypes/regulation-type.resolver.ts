import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';

import { RegulationTypeService } from './regulation-type.service';
import { RegulationTypeListItem } from './regulation-type-list-item.model';

@Injectable()
export class RegulationTypeResolver implements Resolve<RegulationTypeListItem[]> {

  constructor(
    private regulationTypeService: RegulationTypeService
  ) { }

  resolve(route: ActivatedRouteSnapshot) {
    return this.regulationTypeService.getAllRegulationTypes();
  }
}
