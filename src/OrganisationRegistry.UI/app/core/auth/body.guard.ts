import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  CanActivateChild,
  RouterStateSnapshot
} from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { OidcService } from './oidc.service';

@Injectable()
export class BodyGuard implements CanActivate, CanActivateChild {
  constructor(
    private oidcService: OidcService,
  ) { }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.checkPermissions(route);
  }

  public canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.checkPermissions(route);
  }

  private checkPermissions(route: ActivatedRouteSnapshot): Observable<boolean> {
    let bodyIdPart = route.data['bodyGuard'].idPart as string;
    let bodyParams = route.data['bodyGuard'].params as string;

    let params;
    eval('params = ' + bodyParams + ';');

    return this
      .oidcService
      .canEditBody(params[bodyIdPart])
      .map(canEditBody => {
        if (canEditBody)
          return true;

        this.redirectToAuth();
        return false;
      });
  }

  private redirectToAuth() {
    this.oidcService.signIn();
  }
}
