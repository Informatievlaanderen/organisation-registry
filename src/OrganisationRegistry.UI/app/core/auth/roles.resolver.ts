import { Injectable } from "@angular/core";
import { Resolve, ActivatedRouteSnapshot } from "@angular/router";

import { Role } from "./role.model";
import { OidcService, SecurityInfo } from "./oidc.service";
import { map, take } from "rxjs/operators";

@Injectable()
export class RolesResolver implements Resolve<Role[]> {
  constructor(private oidcService: OidcService) {}

  resolve(route: ActivatedRouteSnapshot) {
    return this.oidcService.getOrUpdateValue().pipe(
      map((securityInfo: SecurityInfo) => securityInfo.roles),
      take(1)
    );
  }
}
