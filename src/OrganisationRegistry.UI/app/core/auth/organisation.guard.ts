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

import { AuthService } from './auth.service';
import { Role } from './role.model';
import { OidcService } from './oidc.service';

@Injectable()
export class OrganisationGuard implements CanActivate, CanActivateChild {
  constructor(
    private oidcService: OidcService,
    private router: Router,
    private configurationService: ConfigurationService
  ) { }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.checkPermissions(route);
  }

  public canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.checkPermissions(route);
  }

  private checkPermissions(route: ActivatedRouteSnapshot): Observable<boolean> {
    let organisationIdPart = route.data['organisationGuard'].idPart as string;
    let organisationParams = route.data['organisationGuard'].params as string;

    let params;
    eval('params = ' + organisationParams + ';');

    return this
      .oidcService
      .canEditOrganisation(params[organisationIdPart])
      .map(canEditOrganisation => {
        if (canEditOrganisation)
          return true;

        this.redirectToAuth();
        return false;
      });
  }

  private redirectToAuth() {
    let auth = this.configurationService.authUrl;
    let url = window.location.toString();
    let authUrl = auth + '/secure?returnUrl=' + url;

    // console.log('auth guard - redirect', authUrl);
    window.location.href = authUrl;
  }
}
