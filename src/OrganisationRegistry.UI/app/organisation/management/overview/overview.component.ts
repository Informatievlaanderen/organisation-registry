import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Params, Router} from '@angular/router';

import {Observable} from 'rxjs/Observable';
import {Subscription} from 'rxjs/Subscription';

import {OidcService} from 'core/auth';
import {AlertBuilder, AlertService} from 'core/alert';
import {BaseAlertMessages} from 'core/alertmessages';

import {OrganisationSyncService, OrganisationTermination, TerminationStatus} from 'services/organisationsync';
import {OrganisationInfoService} from "../../../services/organisationinfo";
import {Organisation} from "../../../services/organisations";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationManagementOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationTermination: OrganisationTermination;
  public canEditOrganisation: Observable<boolean>;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie sync');
  private organisationId: string;
  public organisation: Organisation;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationSyncService: OrganisationSyncService,
    private oidcService: OidcService,
    private store: OrganisationInfoService,
    private alertService: AlertService
  ) {
    this.organisationTermination = new OrganisationTermination();
  }

  ngOnInit() {
    let organisationChangedObservable =
      this.store.organisationChanged;

    this.subscriptions.push(organisationChangedObservable
      .subscribe(organisation => {
        if (organisation) {
          this.organisation = organisation;
          this.loadSyncStatus();
        }
      }));

    this.canEditOrganisation = Observable.of(false);
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.canEditOrganisation = this.oidcService.canEditOrganisation(this.organisationId);
      this.store.loadOrganisation(this.organisationId);
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get syncEnabled() {
    return !this.isLoading && this.organisation && this.organisation.kboNumber;
  }

  get syncTerminationEnabled() {
    return !this.isLoading && this.organisation && this.organisation.kboNumber && this.organisationTermination.date;
  }

  sync() {
    this.isLoading = true;

    this.organisationSyncService.sync(this.organisationId)
      .finally(() => this.isLoading = false)
      .subscribe(
        item => {
          this.loadSyncStatus();
          this.alertService.setAlert(
            new AlertBuilder()
              .success()
              .withTitle('Organisatie bijgewerkt!')
              .withMessage('De organisatie werd bijgewerkt vanuit de KBO.')
              .build());
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }

  syncTermination() {
    if (!confirm("Bent u zeker? Deze actie kan niet ongedaan gemaakt worden."))
      return;

    this.isLoading = true;

    this.organisationSyncService.syncTermination(this.organisationId)
      .finally(() => this.isLoading = false)
      .subscribe(
        item => {
          this.loadSyncStatus();
          this.alertService.setAlert(
            new AlertBuilder()
              .success()
              .withTitle('Organisatie bijgewerkt!')
              .withMessage('De organisatie werd stopgezet aan de hand van de data uit de KBO.')
              .build());
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }

  private loadSyncStatus() {
    this.isLoading = true;

    this.organisationSyncService.get(this.organisationId, this.organisation.kboNumber)
      .finally(() => this.isLoading = false)
      .subscribe(
        item => {
          if (item)
            this.organisationTermination = item;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
