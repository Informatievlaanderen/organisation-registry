import { Component, OnDestroy } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { OidcService, Role, SecurityInfo } from "core/auth";
import { ConfigurationService } from "core/configuration";
import { Subscription } from "rxjs/Subscription";
import { map, mergeMap } from "rxjs/operators";
import { Observable } from "rxjs/Observable";
import { of } from "rxjs/observable/of";
import {
  Organisation,
  OrganisationService,
} from "../../services/organisations";
import * as jwt_decode from "jwt-decode";

@Component({
  templateUrl: "callback.template.html",
  styleUrls: ["callback.style.css"],
})
export class CallbackComponent implements OnDestroy {
  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private configurationService: ConfigurationService,
    private organisationService: OrganisationService,
    private oidcService: OidcService
  ) {}

  ngOnInit() {
    this.subscriptions.push(
      this.route.queryParams
        .pipe(
          mergeMap((params: Params) =>
            this.oidcService.exchangeCode(params["code"])
          ),
          mergeMap((token: string) =>
            this.getRedirectUrl(this.oidcService, token)
          )
        )
        .subscribe((url) => {
          window.location.href = url;
        })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  private getRedirectUrl(
    oidcService: OidcService,
    token: string
  ): Observable<string> {
    return oidcService.loadFromServer().pipe(
      mergeMap((si: SecurityInfo) => {
        console.log(si);
        if (
          si.isLoggedIn &&
          si.roles.some((r) => r === Role.DecentraalBeheerder)
        ) {
          return this.organisationService
            .get(jwt_decode(token)["urn:be:vlaanderen:wegwijs:organisation"])
            .pipe(
              map(
                (org: Organisation) =>
                  `${this.configurationService.uiUrl}/#/organisations/${org.id}`
              )
            );
        }
        return of(this.configurationService.uiUrl);
      })
    );
  }
}
