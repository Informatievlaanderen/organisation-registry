import { Component, OnDestroy } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { OidcService, Role, SecurityInfo } from "core/auth";
import { ConfigurationService } from "core/configuration";
import { Subscription } from "rxjs/Subscription";
import {
  catchError,
  concatMap,
  debounceTime,
  defaultIfEmpty,
  map,
  mergeMap,
  take,
  withLatestFrom,
} from "rxjs/operators";
import { Observable } from "rxjs/Observable";
import { empty } from "rxjs/observable/empty";
import { from } from "rxjs/observable/from";
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
    private router: Router,
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
          mergeMap(([token, securityInfo]: [string, SecurityInfo]) => {
            if (
              securityInfo.isLoggedIn &&
              securityInfo.roles.some(
                (r) =>
                  r === Role.DecentraalBeheerder ||
                  r === Role.VoDecentraalBeheerder
              )
            ) {
              // A JWT holds one claim as a string, several claims of the same type as an array.
              const claim = jwt_decode(token)["urn:be:vlaanderen:wegwijs:organisation"];
              const ovoNumbers: string[] = claim ? ([] as string[]).concat(claim) : [];

              // Landing page is best effort: try the organisations one by one, first one that loads wins.
              return from(ovoNumbers).pipe(
                concatMap((ovoNumber) =>
                  this.organisationService
                    .get(ovoNumber, true) // noRedirect: a 404 must not send the user to the 404 page
                    .pipe(catchError(() => empty<Organisation>()))
                ),
                take(1),
                defaultIfEmpty(null)
              );
            }
            return of(null);
          })
        )
        .subscribe((organisation: Organisation) => {
          if (organisation) {
            this.router.navigate(["/", "organisations", organisation.id]);
          } else {
            this.router.navigate(["/"]);
          }
        })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }
}
