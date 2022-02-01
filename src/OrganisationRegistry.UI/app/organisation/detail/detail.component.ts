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
  public enableSync: Observable<boolean>;
  public enableVlimpers: Observable<boolean>;
  private subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private alertService: AlertService,
    private store: OrganisationInfoService,
    private featuresService: FeaturesService,
    private oidcService: OidcService,
  ) {
    this.organisation = new Organisation();
  }

  ngOnInit() {
    this.subscriptions.push(this.store
      .organisationChanged
      .subscribe(org => {
        this.organisation = org;
        this.enableSync = this.oidcService.isLoggedIn && this.oidcService.canEditOrganisation(org);
        this.enableVlimpers = this.oidcService.isLoggedIn && this.oidcService.isVlimpersBeheerder();
      }));

    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.store.loadOrganisation(id);
    });

    this.enableBankAccounts = this.oidcService.isLoggedIn;

    this.enableRegulations = this.featuresService
      .getAllFeatures()
      .map(features => features.regulationsManagement);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(x => x.unsubscribe());
  }
}
