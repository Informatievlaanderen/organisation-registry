import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  CanActivateChild,
  RouterStateSnapshot
} from '@angular/router';

import { Observable } from 'rxjs/Observable';
import {OidcService} from "core/auth";
import {OrganisationInfoService} from "../../services";

@Injectable()
export class OrganisationGuard implements CanActivate, CanActivateChild {
  constructor(
    private oidcService: OidcService,
    private organisationStore: OrganisationInfoService
  ) {
    console.log('GUARD')
  }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.checkPermissions();
  }

  public canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.checkPermissions();
  }

  private checkPermissions(): Observable<boolean> {
    return this
      .oidcService
      .canEditOrganisation(this.organisationStore.organisation)
      .map(canEditOrganisation => {
        if (canEditOrganisation)
          return true;

        this.redirectToAuth();
        return false;
      });
  }

  private redirectToAuth() {
    this.oidcService.signIn();
  }
}
