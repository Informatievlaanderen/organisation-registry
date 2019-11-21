import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { OidcClient } from 'oidc-client';

import { Role, OidcService } from './../../../core/auth';
import { ConfigurationService } from './../../../core/configuration';

import {
  TogglesService,
} from 'services/toggles';
import { Http } from '@angular/http';

@Component({
  selector: 'ww-navbar',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./navbar.style.css'],
  templateUrl: 'navbar.template.html'
})
export class NavbarComponent implements OnInit {
  public isLoggedIn: Observable<boolean>;
  public isOrganisationRegistryBeheerder: Observable<boolean>;
  public isOrganisatieBeheerder: Observable<boolean>;
  public isDeveloper: Observable<boolean>;
  public isOrgaanBeheerder: Observable<boolean>;
  public userName: Observable<string>;
  public role: Observable<string>;
  public enableReporting: Observable<boolean>;

  private securityInfoUrl = `${this.configurationService.apiUrl}/v1/security/info`;

  constructor(
    private oidcService: OidcService,
    private configurationService: ConfigurationService,
    private togglesService: TogglesService,
  ) {
   }

  ngOnInit() {
    this.isLoggedIn = this.oidcService.isLoggedIn;

    const roles = this.oidcService.roles;

    this.isOrganisationRegistryBeheerder =
      this.oidcService.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]);

    this.isOrganisatieBeheerder =
      this.oidcService.hasAnyOfRoles([Role.OrganisatieBeheerder]);

    this.isDeveloper =
      this.oidcService.hasAnyOfRoles([Role.Developer]);

    this.isOrgaanBeheerder =
      this.oidcService.hasAnyOfRoles([Role.OrgaanBeheerder]);

    this.userName = this.oidcService.userName;

    this.role = roles.map(roles => {
      let role = '';
      if (roles.indexOf(Role.OrganisationRegistryBeheerder) !== -1) {
        role = 'Beheerder';
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

    this.enableReporting = this.togglesService
      .getAllToggles()
      .map(toggles => toggles.enableReporting);
  }

  loginClicked(): void {
    this.oidcService.signIn();
  }

  logoutClicked(): void {
    this.oidcService.signOut();
  }
}
