import {Component, OnInit, ChangeDetectionStrategy, Input} from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { Role, OidcService } from 'core/auth';
import { ConfigurationService } from 'core/configuration';

import {FeaturesService} from "../../../services/features";
import {Environments} from "../../../environments";

@Component({
  selector: 'ww-navbar',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./navbar.style.css'],
  templateUrl: 'navbar.template.html'
})
export class NavbarComponent implements OnInit {
  @Input('environment') environment: string;

  public isLoggedIn: Observable<boolean>;
  public isOrganisationRegistryBeheerder: Observable<boolean>;
  public isVlimpersBeheerder: Observable<boolean>;
  public isOrganisatieBeheerder: Observable<boolean>;
  public isDeveloper: Observable<boolean>;
  public isOrgaanBeheerder: Observable<boolean>;
  public userName: Observable<string>;
  public role: Observable<string>;
  public showEnvironment: boolean;
  constructor(
    private oidcService: OidcService,
  ) {
   }

  ngOnInit() {
    this.showEnvironment = this.environment != Environments.production;

    this.isLoggedIn = this.oidcService.isLoggedIn;

    const roles = this.oidcService.roles;

    this.isOrganisationRegistryBeheerder =
      this.oidcService.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]);

    this.isVlimpersBeheerder =
      this.oidcService.hasAnyOfRoles([Role.VlimpersBeheerder]);

    this.isOrganisatieBeheerder =
      this.oidcService.hasAnyOfRoles([Role.OrganisatieBeheerder]);

    this.isDeveloper =
      this.oidcService.hasAnyOfRoles([Role.Developer]);

    this.isOrgaanBeheerder =
      this.oidcService.hasAnyOfRoles([Role.OrgaanBeheerder]);

    this.userName = this.oidcService.userName;

    this.role = roles.map(roles => {
      let role = '';
      console.log(roles);
      if (roles.indexOf(Role.OrganisationRegistryBeheerder) !== -1) {
        role = 'Beheerder';
      } else if (roles.indexOf(Role.VlimpersBeheerder) !== -1) {
        role = 'Vlimpersbeheerder';
      } else if (roles.indexOf(Role.OrganisatieBeheerder) !== -1) {
        role = 'Invoerder';
      } else if (roles.indexOf(Role.OrgaanBeheerder) !== -1) {
        role = 'Orgaanbeheerder';
      }

      if (roles.indexOf(Role.Developer) !== -1) {
        role = role + ' | Ontwikkelaar';
      }

      return role;
    });
  }

  loginClicked(): void {
    this.oidcService.signIn();
  }

  logoutClicked(): void {
    this.oidcService.signOut();
  }
}
