import {ChangeDetectionStrategy, Component, Input, OnInit} from '@angular/core';

import {Observable} from 'rxjs/Observable';

import {OidcService, Role} from 'core/auth';
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
      this.oidcService.hasAnyOfRoles([Role.AlgemeenBeheerder]);

    this.isVlimpersBeheerder =
      this.oidcService.hasAnyOfRoles([Role.VlimpersBeheerder]);

    this.isOrganisatieBeheerder =
      this.oidcService.hasAnyOfRoles([Role.DecentraalBeheerder]);

    this.isDeveloper =
      this.oidcService.hasAnyOfRoles([Role.Developer]);

    this.isOrgaanBeheerder =
      this.oidcService.hasAnyOfRoles([Role.OrgaanBeheerder]);

    this.userName = this.oidcService.userName;

    this.role = roles.map(x => {
      let role = '';
      if (x.indexOf(Role.AlgemeenBeheerder) !== -1) {
        role = 'Algemeen beheerder';
      } else if (x.indexOf(Role.VlimpersBeheerder) !== -1) {
        role = 'Vlimpers beheerder';
      } else if (x.indexOf(Role.DecentraalBeheerder) !== -1) {
        role = 'Decentraal beheerder';
      } else if (x.indexOf(Role.RegelgevingBeheerder) !== -1) {
        role = 'Regelgeving en deugdelijk bestuur beheerder';
      } else if (x.indexOf(Role.OrgaanBeheerder) !== -1) {
        role = 'Orgaan beheerder';
      }

      if (x.indexOf(Role.Developer) !== -1) {
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
