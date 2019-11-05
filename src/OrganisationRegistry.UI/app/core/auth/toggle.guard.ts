import { Toggles } from '../../services/toggles/toggles.model';
import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  CanActivateChild,
  Router,
  RouterStateSnapshot
} from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { ConfigurationService } from './../configuration';

import { TogglesService } from 'services/toggles';
import { Role } from './role.model';

@Injectable()
export class ToggleGuard implements CanActivate, CanActivateChild {
  constructor(
    private togglesService: TogglesService,
    private router: Router,
    private configurationService: ConfigurationService
  ) { }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.isBankAccountsToggleEnabled(route);
  }

  public canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.isBankAccountsToggleEnabled(route);
  }

  private isBankAccountsToggleEnabled(route: ActivatedRouteSnapshot): Observable<boolean> {
    let desiredToggles = route.data['toggles'] as Array<string>;

    return this
      .togglesService
      .getAllToggles()
      .map(toggle =>
        desiredToggles.every(dt => toggle[dt]));
  }
}
