import { Injectable, OnDestroy } from "@angular/core";
import { Http, Response } from "@angular/http";
import { Observable } from "rxjs/Observable";
import { BehaviorSubject } from "rxjs/BehaviorSubject";

import { ConfigurationService } from "../configuration";
import { HeadersBuilder } from "../http";

import { User } from "./user.model";

import { Role } from "./role.model";
import { OidcClient } from "oidc-client";
import { Subscription } from "rxjs/Subscription";
import {
  catchError,
  distinctUntilChanged,
  map,
  mergeMap,
  shareReplay,
  tap,
} from "rxjs/operators";

export function hasAnyOfRoles(
  securityInfo: SecurityInfo,
  ...desiredRoles: Array<Role>
): boolean {
  for (let userRole of securityInfo.roles) {
    if (desiredRoles.findIndex((x) => x === userRole) > -1) return true;
  }

  return securityInfo.roles.findIndex((x) => x === Role.Developer) > -1;
}

export function isOrganisatieBeheerderFor(
  securityInfo: SecurityInfo,
  organisationId: string
): boolean {
  return (
    hasAnyOfRoles(securityInfo, Role.DecentraalBeheerder) &&
    securityInfo.organisationIds.findIndex((x) => x === organisationId) > -1
  );
}

export interface SecurityInfo {
  isLoggedIn: boolean;
  userName: string;
  roles: Array<Role>;
  ovoNumbers: Array<string>;
  organisationIds: Array<string>;
  bodyIds: Array<string>;
  refreshtoken: number;
  expires: number;
}

function createSecurityInfo(
  user: User = null,
  refreshToken: number = 1000
): SecurityInfo {
  return {
    isLoggedIn: !!user,
    bodyIds: user ? user.bodyIds : new Array<string>(),
    organisationIds: user ? user.organisationIds : new Array<string>(),
    ovoNumbers: user ? user.ovoNumbers : new Array<string>(),
    userName: user ? user.userName : "",
    roles: user ? user.roles : new Array<Role>(),
    expires: new Date().getTime() - 60 * 1000,
    refreshtoken: refreshToken,
  };
}

@Injectable()
export class OidcService implements OnDestroy {
  private securityUrl = `${this.configurationService.apiUrl}/v1/security`;
  private securityInfoUrl = `${this.configurationService.apiUrl}/v1/security/info`;

  private securityInfoSubject = new BehaviorSubject<SecurityInfo>(
    createSecurityInfo()
  );
  private securityInfo$ = this.securityInfoSubject.asObservable();

  private client: OidcClient;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) {
    this.updateFromServer().subscribe();
    this.subscriptions.push(
      this.http.get(this.securityInfoUrl).subscribe((r) => {
        const data = r.json();
        const settings = {
          authority: data.authority,
          metadata: {
            issuer: data.issuer,
            authorization_endpoint: data.authorizationEndpoint,
            userinfo_endpoint: data.userInfoEndPoint,
            end_session_endpoint: data.endSessionEndPoint,
            jwks_uri: data.jwksUri,
          },
          signing_keys: ["RS256"],
          client_id: data.clientId,
          redirect_uri: data.redirectUri,
          post_logout_redirect_uri: data.postLogoutRedirectUri,
          response_type: "code",
          scope: "openid profile vo iv_wegwijs",
          filterProtocolClaims: true,
          loadUserInfo: true,
          query_status_response_type: "code",
        };
        this.client = new OidcClient(settings);
      })
    );
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  exchangeCode(code: string): Observable<[string, SecurityInfo]> {
    const url = `${
      this.securityUrl
    }/exchange?code=${code}&verifier=${localStorage.getItem("verifier")}`;

    let headers = new HeadersBuilder().json().build();

    return this.http.get(url, { headers: headers }).pipe(
      catchError(OidcService.handleError),
      map((res: Response) => {
        localStorage.setItem("token", res.json());
        return localStorage.getItem("token");
      }),
      mergeMap((token) =>
        this.updateFromServer().pipe(
          map((securityInfo: SecurityInfo) => [token, securityInfo])
        )
      )
    );
  }

  public signIn() {
    this.client
      .createSigninRequest()
      .then((req) => {
        localStorage.setItem("verifier", req.state.code_verifier);
        window.location.href = req.url;
      })
      .catch((err) => {
        console.error("Could not create signin request!", err, err.request);
      });
  }

  public signOut() {
    localStorage.removeItem("token");
    localStorage.removeItem("verifier");
    this.client
      .createSignoutRequest()
      .then((req) => {
        window.location.href = req.url;
      })
      .catch((err) => {
        console.error("Could not create signout request!", err, err.request);
      });
  }

  public get isLoggedIn(): Observable<boolean> {
    return this.getOrUpdateValue().map((user) => user.isLoggedIn);
  }

  public get userName(): Observable<string> {
    return this.getOrUpdateValue().map((user) => user.userName);
  }

  public get roles(): Observable<Array<Role>> {
    return this.getOrUpdateValue().map((user) => user.roles);
  }

  public get ovoNumbers(): Observable<Array<string>> {
    return this.getOrUpdateValue().map((user) => user.ovoNumbers);
  }

  public get organisationIds(): Observable<Array<string>> {
    return this.getOrUpdateValue().map((user) => user.organisationIds);
  }

  public get bodyIds(): Observable<Array<string>> {
    return this.getOrUpdateValue().map((user) => user.bodyIds);
  }

  public canEditBody(bodyId): Observable<boolean> {
    return this.securityInfo$.pipe(
      map((securityInfo: SecurityInfo) => {
        if (
          hasAnyOfRoles(
            securityInfo,
            Role.AlgemeenBeheerder,
            Role.CjmBeheerder,
            Role.OrgaanBeheerder
          )
        )
          return true;
        if (
          hasAnyOfRoles(securityInfo, Role.DecentraalBeheerder) &&
          securityInfo.bodyIds.some((x) => bodyId === x)
        )
          return true;

        return false;
      })
    );
  }

  public hasAnyOfRoles(...desiredRoles: Array<Role>): Observable<boolean> {
    return this.roles
      .map((roles) => {
        for (let userRole of roles) {
          if (desiredRoles.findIndex((x) => x === userRole) > -1) return true;
        }

        return roles.findIndex((x) => x === Role.Developer) > -1;
      })
      .catch((_) => {
        return Observable.of(false);
      });
  }

  public getOrUpdateValue(): Observable<SecurityInfo> {
    return this.securityInfo$.pipe(
      tap((securityInfo: SecurityInfo) => {
        if (securityInfo.expires > new Date().getTime()) {
          this.updateFromServer().subscribe();
        }
      })
    );
  }

  public updateFromServer(): Observable<SecurityInfo> {
    return this.getUser().pipe(
      map((user: User) =>
        createSecurityInfo(
          user,
          this.securityInfoSubject.getValue().refreshtoken + 1
        )
      ),
      tap((securityInfo: SecurityInfo) => {
        this.securityInfoSubject.next(securityInfo);
      })
    );
  }

  private getUser(): Observable<User> {
    const url = `${this.securityUrl}`;

    let headers = new HeadersBuilder().json().build();

    return this.http
      .get(url, { headers: headers })
      .pipe(map(OidcService.toUser), catchError(OidcService.handleError));
  }

  private static toUser(res: Response): User {
    let body = res.json();
    return new User(
      body.userName,
      body.roles,
      body.ovoNumbers,
      body.organisationIds,
      body.bodyIds
    );
  }

  private static handleError(error: any): Observable<any> {
    if (error.status === 401) return Observable.throw(error);

    // In a real world app, we might use a remote logging infrastructure
    // We'd also dig deeper into the error to get a better message
    let errMsg = error.message
      ? error.message
      : error.status
      ? `${error.status} - ${error.statusText}`
      : "Server error";

    console.error("An error occurred", errMsg); // log to console instead
    return Observable.throw(errMsg);
  }
}
