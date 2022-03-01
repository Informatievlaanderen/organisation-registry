import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router} from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';

import { OrganisationInfoService} from 'services/organisationinfo';
import { Organisation } from 'services/organisations';
import {OrganisationVlimpersService} from "../../../services/organisationvlimpers";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationVlimpersOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = false;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie Vlimpersbeheer');
  private organisationId: string;
  public organisation: Organisation = new Organisation();

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationVlimpersService: OrganisationVlimpersService,
    private oidcService: OidcService,
    private store: OrganisationInfoService,
    private alertService: AlertService
  ) {
  }

  ngOnInit() {
    let organisationChangedObservable =
      this.store.organisationChanged;

    this.subscriptions.push(organisationChangedObservable
      .subscribe(organisation => {
        if (organisation) {
          this.organisation = organisation;
        }
      }));

    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.store.loadOrganisation(this.organisationId);
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get syncEnabled() {
    return !this.isLoading && this.organisation && this.organisation.kboNumber;
  }

  public placeUnderVlimpersManagement() {
    this.isLoading = true;

    this.subscriptions.push(this.organisationVlimpersService.placeUnderVlimpersManagement(this.organisationId)
      .finally(() => this.isLoading = false)
      .subscribe(
        item => {
          this.store.loadOrganisation(this.organisationId);
          this.alertService.setAlert(
            new AlertBuilder()
              .success()
              .withTitle('Organisatie bijgewerkt!')
              .withMessage('De organisatie werd onder Vlimpersbeheer geplaatst.')
              .build());
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build())));
  }

  public removeFromVlimpersManagement() {
    this.isLoading = true;

    this.subscriptions.push(this.organisationVlimpersService.removeFromVlimpersManagement(this.organisationId)
      .finally(() => this.isLoading = false)
      .subscribe(
        item => {
          this.store.loadOrganisation(this.organisationId);
          this.alertService.setAlert(
            new AlertBuilder()
              .success()
              .withTitle('Organisatie bijgewerkt!')
              .withMessage('De organisatie werd uit Vlimpersbeheer gehaald.')
              .build());
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build())));
  }
}
