import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params} from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService } from 'core/alert';

import { OidcService } from 'core/auth';

import { OrganisationInfoService } from 'services/organisationinfo';
import { Organisation } from 'services/organisations';
import {FeaturesService} from "services/features";
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: ['detail.style.css']
})
export class OrganisationDetailComponent implements OnInit, OnDestroy {
  public organisation: Organisation;
  public enableBankAccounts: Observable<boolean>;
  public enableRegulations: Observable<boolean>;
  public showKboManagement: Observable<boolean>;
  public showVlimpers: Observable<boolean>;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private alertService: AlertService,
    private featuresService: FeaturesService,
    private oidcService: OidcService,
    public store: OrganisationInfoService,
  ) {
    this.organisation = new Organisation();
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.store.loadOrganisation(id);
    });

    this.subscriptions.push(
      this.store.organisationChanged.subscribe(org => this.organisation = org)
    );

    this.enableBankAccounts = this.oidcService.isLoggedIn;

    this.enableRegulations = this.featuresService
      .getAllFeatures()
      .map(features => features.regulationsManagement);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(x => x.unsubscribe());
  }
}
