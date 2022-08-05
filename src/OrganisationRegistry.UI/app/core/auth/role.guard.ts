import { Injectable } from "@angular/core";
import {
  ActivatedRouteSnapshot,
  CanActivate,
  CanActivateChild,
  RouterStateSnapshot,
} from "@angular/router";

import { Observable } from "rxjs/Observable";

import { Role } from "./role.model";
import { OidcService } from "./oidc.service";

@Injectable()
export class RoleGuard implements CanActivate, CanActivateChild {
  constructor(private oidcService: OidcService) {}

  public canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.checkPermissions(route);
  }

  public canActivateChild(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.checkPermissions(route);
  }

  private checkPermissions(route: ActivatedRouteSnapshot): Observable<boolean> {
    let desiredRoles = route.data["roles"] as Array<Role>;

    return this.oidcService.hasAnyOfRoles(...desiredRoles).map((hasRole) => {
      if (hasRole) return true;

      this.redirectToAuth();
      return false;
    });
  }

  private redirectToAuth() {
    this.oidcService.signIn();
  }
}
