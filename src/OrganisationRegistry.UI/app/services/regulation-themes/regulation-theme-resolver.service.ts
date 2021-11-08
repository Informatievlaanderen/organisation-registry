import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';

import { RegulationThemeService } from './regulation-theme.service';
import { RegulationThemeListItem } from './regulation-theme-list-item.model';

@Injectable()
export class RegulationThemeResolver implements Resolve<RegulationThemeListItem[]> {

  constructor(
    private regulationThemeService: RegulationThemeService
  ) { }

  resolve(route: ActivatedRouteSnapshot) {
    return this.regulationThemeService.getAllRegulationThemes();
  }
}
