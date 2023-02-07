import {Component, OnInit, OnDestroy} from "@angular/core";
import {ActivatedRoute, Router} from "@angular/router";

import {Subscription} from "rxjs/Subscription";

import {OidcService} from "core/auth";
import {AlertService} from "core/alert";

import {BodyService} from "services/bodies";
import {OrganisationService} from "services/organisations";

import {SearchResult} from "shared/components/form/form-group-autocomplete";

import {CreateChildAlerts} from "./create.alerts";

import {CreateBodyFormValues} from "./create-body-form";
import {finalize, take} from "rxjs/operators";

@Component({
  templateUrl: "create.template.html",
  styleUrls: ["create.style.css"],
})
export class CreateBodyComponent implements OnInit, OnDestroy {
  public isBusy: boolean = false;
  public body;
  public organisation: SearchResult;

  private readonly createAlerts = new CreateChildAlerts();
  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bodyService: BodyService,
    private organisationService: OrganisationService,
    private alertService: AlertService,
    private oidcService: OidcService
  ) {
  }

  ngOnInit() {
    this.body = new CreateBodyFormValues();
    this.subscriptions.push(
      this.route.queryParams.subscribe((params) => {
        if (params["organisationId"]) {
          this.body.organisationId = params["organisationId"];
          this.isBusy = true;
          this.subscriptions.push(
            this.organisationService
              .get(this.body.organisationId)
              .finally(() => (this.isBusy = false))
              .subscribe(
                (result) =>
                  (this.organisation = new SearchResult(result.id, result.name))
              )
          );
        }
      })
    );
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  create(value: CreateBodyFormValues) {
    this.isBusy = true;
    this.subscriptions.push(
      this.bodyService
        .create(value)
        .finally(() => (this.isBusy = false))
        .subscribe(
          (result) => this.onCreateSuccess(result, value),
          (error) =>
            this.alertService.setAlert(this.createAlerts.saveError(error))
        )
    );
  }

  private onCreateSuccess(result, value) {
    if (result) {
      const sub = this.oidcService.updateSecurityInfo().pipe(
        take(1),
        finalize(() => sub.unsubscribe()))
        .subscribe();

      this.router.navigate(["./../", value.id], {relativeTo: this.route});

      let bodyUrl = this.router.serializeUrl(
        this.router.createUrlTree(["./../", value.id], {
          relativeTo: this.route,
        })
      );

      this.alertService.setAlert(this.createAlerts.saveSuccess(value, bodyUrl));
    }
  }
}
