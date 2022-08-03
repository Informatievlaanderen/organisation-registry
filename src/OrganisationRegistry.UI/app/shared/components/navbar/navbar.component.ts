import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
} from "@angular/core";

import { Observable } from "rxjs/Observable";

import { OidcService, Role } from "core/auth";
import { Environments } from "../../../environments";
import { map } from "rxjs/operators";

@Component({
  selector: "ww-navbar",
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ["./navbar.style.css"],
  templateUrl: "navbar.template.html",
})
export class NavbarComponent implements OnInit {
  @Input("environment") environment: string;

  public isLoggedIn: Observable<boolean>;
  public isOrganisationRegistryBeheerder: Observable<boolean>;
  public isVlimpersBeheerder: Observable<boolean>;
  public isOrganisatieBeheerder: Observable<boolean>;
  public isDeveloper: Observable<boolean>;
  public isOrgaanBeheerder: Observable<boolean>;
  public userName: Observable<string>;
  public roleName$: Observable<string>;
  public showEnvironment: boolean = this.environment != Environments.production;

  constructor(private oidcService: OidcService) {}

  ngOnInit() {
    this.isLoggedIn = this.oidcService.isLoggedIn;

    this.isOrganisationRegistryBeheerder = this.oidcService.hasAnyOfRoles([
      Role.AlgemeenBeheerder,Role.CjmBeheerder
    ]);

    this.isVlimpersBeheerder = this.oidcService.hasAnyOfRoles([
      Role.VlimpersBeheerder,
    ]);

    this.isOrganisatieBeheerder = this.oidcService.hasAnyOfRoles([
      Role.DecentraalBeheerder,
    ]);

    this.isDeveloper = this.oidcService.hasAnyOfRoles([Role.Developer]);

    this.isOrgaanBeheerder = this.oidcService.hasAnyOfRoles([
      Role.OrgaanBeheerder,
    ]);

    this.userName = this.oidcService.userName;

    this.roleName$ = this.oidcService.roles.pipe(
      map((roles: Role[]) => {
        let role = NavbarComponent.getRoleName(roles);
        return NavbarComponent.appendDeveloperIfInRole(role, roles);
      })
    );
  }

  private static getRoleName(roles: Role[]): string {
    if (roles.indexOf(Role.AlgemeenBeheerder) !== -1) {
      return "Algemeen beheerder";
    }
    if (roles.indexOf(Role.VlimpersBeheerder) !== -1) {
      return "Vlimpers beheerder";
    }
    if (roles.indexOf(Role.DecentraalBeheerder) !== -1) {
      return "Decentraal beheerder";
    }
    if (roles.indexOf(Role.RegelgevingBeheerder) !== -1) {
      return "Regelgeving en deugdelijk bestuur beheerder";
    }
    if (roles.indexOf(Role.OrgaanBeheerder) !== -1) {
      return "Orgaan beheerder";
    }
    if (roles.indexOf(Role.CjmBeheerder) !== -1) {
      return "Cjm beheerder";
    }
  }

  private static appendDeveloperIfInRole(s: string, roles: Role[]): string {
    if (roles.indexOf(Role.Developer) !== -1) {
      return s + " | Ontwikkelaar";
    }
    return s;
  }

  loginClicked(): void {
    this.oidcService.signIn();
  }

  logoutClicked(): void {
    this.oidcService.signOut();
  }
}
