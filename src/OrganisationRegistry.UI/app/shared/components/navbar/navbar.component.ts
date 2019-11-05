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
  private client: OidcClient;

  private securityInfoUrl = `${this.configurationService.apiUrl}/v1/security/info`;

  constructor(
    private oidcService: OidcService,
    private configurationService: ConfigurationService,
    private togglesService: TogglesService,
    http: Http
  ) {
      http.get(this.securityInfoUrl)
        .subscribe(r => {
          var data = r.json();
          const settings = {
            authority: data.authority,
            metadata: {
              issuer: data.issuer,
              authorization_endpoint: data.authorizationEndpoint,
              userinfo_endpoint: data.userInfoEndPoint,
              end_session_endpoint: data.endSessionEndPoint,
              jwks_uri: data.jwksUri,
            },
            signing_keys: ['RS256'],
            client_id: data.clientId,
            redirect_uri: data.redirectUri,
            post_logout_redirect_uri: data.postLogoutRedirectUri,
            response_type: 'code',
            scope: 'openid profile vo iv_wegwijs',
            filterProtocolClaims: true,
            loadUserInfo: true,
          }
          this.client = new OidcClient(settings);
        })
   }

  ngOnInit() {
    this.isLoggedIn = this.oidcService.isLoggedIn;

    const roles = this.oidcService.roles;

    this.isOrganisationRegistryBeheerder = roles.map(roles => {
      return (roles.indexOf(Role.OrganisationRegistryBeheerder) !== -1) ;
    });

    this.isOrganisatieBeheerder = roles.map(roles => {
      return (roles.indexOf(Role.OrganisatieBeheerder) !== -1) ;
    });

    this.isDeveloper = roles.map(roles => {
      return (roles.indexOf(Role.Developer) !== -1) ;
    });

    this.isOrgaanBeheerder = roles.map(roles => {
      return (roles.indexOf(Role.OrgaanBeheerder) !== -1) ;
    });

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
    this.client
      .createSigninRequest({
        state: {
          bar: 15,
        },
      })
      .then((req) => {
        window.location.href = req.url;
      }).catch((err) => {
        console.log('Shriek!', err);
        console.log('Shriek!', err.request);
      });
  }

  logoutClicked(): void {
    localStorage.removeItem('token');
    this.client
      .createSignoutRequest({
        state: {
          bar: 15,
        },
      })
      .then((req) => {
        window.location.href = req.url;
      }).catch((err) => {
        console.log('Shriek!', err);
        console.log('Shriek!', err.request);
      });
  }
}
