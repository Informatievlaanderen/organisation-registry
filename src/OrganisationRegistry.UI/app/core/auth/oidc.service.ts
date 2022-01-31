import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Subject } from 'rxjs/Subject';

import { ConfigurationService } from './../configuration';
import { HeadersBuilder } from './../http';

import { User } from './user.model';

import { Role } from './role.model';
import {OidcClient} from "oidc-client";
import {Organisation} from "../../services/organisations";

interface SecurityInfo {
  isLoggedIn: boolean;
  userName: string;
  roles: Array<Role>;
  ovoNumbers: Array<string>;
  organisationIds: Array<string>;
  bodyIds: Array<string>;
  refreshtoken: number;
  expires: number;
}

@Injectable()
export class OidcService {
  private securityUrl = `${this.configurationService.apiUrl}/v1/security`;
  private securityInfoUrl = `${this.configurationService.apiUrl}/v1/security/info`;
  private cacheTimeInMs: number = 60000;

  private storage: SecurityInfo = {
    isLoggedIn: false,
    userName: '',
    roles: new Array<Role>(),
    ovoNumbers: new Array<string>(),
    organisationIds: new Array<string>(),
    bodyIds: new Array<string>(),
    refreshtoken: 1000,
    expires: new Date().getTime() - (60 * 1000)
  };

  private data$ = new BehaviorSubject(this.storage);
  private request$ = new Subject<SecurityInfo>();
  private client: OidcClient;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
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
          query_status_response_type: 'code'
        };
        this.client = new OidcClient(settings);
      });

    this.request$
      .exhaustMap(this.loadFromServer.bind(this))
      .share()
      .startWith(this.storage)
      .subscribe(
        x => { this.data$.next(x as SecurityInfo); },
        err => { this.data$.next(err as SecurityInfo); },
        () => {});

    this.data$.subscribe(this.saveToStorage.bind(this));
  }

  exchangeCode(code: string, configurationService: ConfigurationService) {
    const url = `${this.securityUrl}/exchange?code=${code}&verifier=${localStorage.getItem('verifier')}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .catch(this.handleError)
      .subscribe(res => {
        const token = res.json();
        localStorage.setItem('token', token);
        window.location.href = configurationService.uiUrl;
      })
  }

  public signIn() {
    this.client
      .createSigninRequest()
      .then((req) => {
        localStorage.setItem('verifier', req.state.code_verifier);
        window.location.href = req.url;
      })
      .catch((err) => {
        console.error('Could not create signin request!', err, err.request);
      });
  }

  public signOut() {
    localStorage.removeItem('token');
    localStorage.removeItem('verifier');
    this.client
      .createSignoutRequest()
      .then((req) => {
        window.location.href = req.url;
      }).catch((err) => {
      console.error('Could not create signout request!', err, err.request);
    });
  }

  public get isLoggedIn(): Observable<boolean> {
    return this.getOrUpdateValue().map(user => user.isLoggedIn);
  }

  public get userName(): Observable<string> {
    return this.getOrUpdateValue().map(user => user.userName);
  }

  public get roles(): Observable<Array<Role>> {
    return this.getOrUpdateValue().map(user => user.roles);
  }

  public get ovoNumbers(): Observable<Array<string>> {
    return this.getOrUpdateValue().map(user => user.ovoNumbers);
  }

  public get organisationIds(): Observable<Array<string>> {
    return this.getOrUpdateValue().map(user => user.organisationIds);
  }

  public get bodyIds(): Observable<Array<string>> {
    return this.getOrUpdateValue().map(user => user.bodyIds);
  }

  public isVlimpersBeheerder(): Observable<boolean> {
    let wegwijsBeheerderCheck = this.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]);
    let vlimpersBeheerderCheck = this.hasAnyOfRoles([Role.VlimpersBeheerder]);

    return Observable.zip(wegwijsBeheerderCheck, vlimpersBeheerderCheck)
      .map(zipped => {
        let isOrganisationRegistryBeheerder = zipped[0];
        let isVlimpersBeheerder = zipped[1];

        if (isOrganisationRegistryBeheerder || isVlimpersBeheerder)
          return true;

        return false;
      })
      .catch(err => {
        return Observable.of(false);
      });
  }

  public canEditOrganisation(organisation : Organisation): Observable<boolean> {
    let wegwijsBeheerderCheck = this.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]);
    let vlimpersBeheerderCheck = this.hasAnyOfRoles([Role.VlimpersBeheerder]);
    let organisatieBeheerderCheck = this.hasAnyOfRoles([Role.OrganisatieBeheerder]);

    return Observable.zip(wegwijsBeheerderCheck, vlimpersBeheerderCheck, organisatieBeheerderCheck, this.organisationIds)
      .map(zipped => {
        let isOrganisationRegistryBeheerder = zipped[0];
        let isVlimpersBeheerder = zipped[1];
        let isOrganisatieBeheerder = zipped[2];
        let organisationIds = zipped[3];

        if (isOrganisationRegistryBeheerder)
          return true;

        if (organisation.underVlimpersManagement && isVlimpersBeheerder)
          return true;

        if (isOrganisatieBeheerder && organisationIds.findIndex(x => x === organisation.id) > -1)
          return true;

        return false;
      })
      .catch(err => {
        return Observable.of(false);
      });
  }

  public canAddBody(organisationId): Observable<boolean> {
    let wegwijsBeheerderCheck = this.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]);
    let orgaanBeheerderCheck = this.hasAnyOfRoles([Role.OrgaanBeheerder]);
    let organisatieBeheerderCheck = this.hasAnyOfRoles([Role.OrganisatieBeheerder]);

    return Observable.zip(wegwijsBeheerderCheck, orgaanBeheerderCheck, organisatieBeheerderCheck, this.organisationIds)
      .map(zipped => {
        let isOrganisationRegistryBeheerder = zipped[0];
        let isOrgaanBeheerder = zipped[1];
        let isOrganisatieBeheerder = zipped[2];
        let organisationIds = zipped[3];

        if (isOrganisationRegistryBeheerder)
          return true;

        if (isOrgaanBeheerder)
          return true;

        if (isOrganisatieBeheerder && organisationIds.findIndex(x => x === organisationId) > -1)
            return true;

        return false;
      })
      .catch(err => {
        return Observable.of(false);
      });
  }

  public canEditBody(bodyId): Observable<boolean> {
    let wegwijsBeheerderCheck = this.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]);
    let orgaanBeheerderCheck = this.hasAnyOfRoles([Role.OrgaanBeheerder]);
    let organisatieBeheerderCheck = this.hasAnyOfRoles([Role.OrganisatieBeheerder]);

    return Observable.zip(wegwijsBeheerderCheck, orgaanBeheerderCheck, organisatieBeheerderCheck, this.bodyIds)
      .map(zipped => {
        let isOrganisationRegistryBeheerder = zipped[0];
        let isOrgaanBeheerder = zipped[1];
        let isOrganisatieBeheerder = zipped[2];
        let bodyIds = zipped[3];

        if (isOrganisationRegistryBeheerder)
          return true;

        if (isOrgaanBeheerder)
          return true;

        if (isOrganisatieBeheerder && bodyIds.findIndex(x => x === bodyId) > -1)
            return true;

        return false;
      })
      .catch(err => {
        return Observable.of(false);
      });
  }

  public hasAnyOfRoles(desiredRoles: Array<Role>): Observable<boolean> {
    return this.roles
      .map(roles => {
        for (let userRole of roles) {
          if (desiredRoles.findIndex(x => x === userRole) > -1)
            return true;
        }

        return roles.findIndex(x => x === Role.Developer) > -1;
      })
      .catch(err => {
        return Observable.of(false);
      });
  }

  public resetSecurityCache() {
    this.request$.next(null);
  }

  private getOrUpdateValue(): Observable<SecurityInfo> {
    return this.data$.take(1)
      .filter((value, index) => {
        return value && value.refreshtoken > 0;
      })
      .switchMap(data => {
        if (data.expires > new Date().getTime()) {
          return this.data$.take(1);
        } else {
          // console.log('expired ...');
          this.request$.next(null);
          return this.data$.skip(1).take(1);
        }
      });
  }

  private loadFromServer(): Observable<SecurityInfo> {
    // console.log('loading from server request');
    return this.get()
      .map(user => {
        return {
          isLoggedIn: true,
          userName: user.userName,
          roles: user.roles,
          ovoNumbers: user.ovoNumbers,
          organisationIds: user.organisationIds,
          bodyIds: user.bodyIds,
          refreshtoken: this.storage.refreshtoken + 1,
          expires: new Date().getTime() + this.cacheTimeInMs
        } as SecurityInfo;
      })
      .catch(err => {
        return Observable.throw({
          isLoggedIn: false,
          userName: '',
          roles: new Array<Role>(),
          ovoNumbers: new Array<string>(),
          organisationIds: new Array<string>(),
          bodyIds: new Array<string>(),
          refreshtoken: this.storage.refreshtoken + 1,
          expires: new Date().getTime() + this.cacheTimeInMs
        } as SecurityInfo);
      });
  }

  private loadFromStorage(): Observable<SecurityInfo> {
    return Observable.of(this.storage);
  }

  private saveToStorage(data: SecurityInfo) {
    // console.log('saving: ', data);
    this.storage = data;
    return data;
  }

  private get(): Observable<User> {
    const url = `${this.securityUrl}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toUser)
      .catch(this.handleError);
  }

  private toUser(res: Response): User {
    let body = res.json();
    return new User(body.userName, body.roles, body.ovoNumbers, body.organisationIds, body.bodyIds);
  }

  private handleError(error: any): Observable<any> {
    if (error.status === 401)
      return Observable.throw(error);

    // In a real world app, we might use a remote logging infrastructure
    // We'd also dig deeper into the error to get a better message
    let errMsg = (error.message)
      ? error.message
      : error.status
        ? `${error.status} - ${error.statusText}`
        : 'Server error';

    console.error('An error occurred', errMsg); // log to console instead
    return Observable.throw(errMsg);
  }
}
