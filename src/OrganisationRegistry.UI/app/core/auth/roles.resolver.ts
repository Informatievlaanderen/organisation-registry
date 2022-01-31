import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';

import { Role } from './role.model';
import { OidcService } from './oidc.service';

@Injectable()
export class RolesResolver implements Resolve<Role[]> {

  constructor(
    private oidcService: OidcService
  ) { }

  resolve(route: ActivatedRouteSnapshot) {
    return this.oidcService.roles;
  }
}
