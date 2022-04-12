import {Component, OnDestroy, OnInit} from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { Alert, AlertService, AlertType } from 'core/alert';
import { PagedEvent, PagedResult, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';
import { OidcService, Role } from 'core/auth';

import { Body, BodyService } from 'services/bodies';

import {
  BodyParticipationReportFilter,
  BodyParticipationReportListItem,
  BodyParticipationReportService,
  BodyParticipationReportTotals,
  Compliance
} from 'services/reports/body-participation';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class BodyParticipationOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public bodyId: string;
  public body: Body;
  public bodyParticipations: PagedResult<BodyParticipationReportListItem> = new PagedResult<BodyParticipationReportListItem>();
  public bodyParticipationTotals: BodyParticipationReportTotals = new BodyParticipationReportTotals();
  public canManageMep: Observable<boolean>;

  private filter: BodyParticipationReportFilter = new BodyParticipationReportFilter();
  private currentSortBy: string = 'bodyseatTypeName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private alertService: AlertService,
    private bodyService: BodyService,
    private bodyParticipationReportService: BodyParticipationReportService,
    private oidcService: OidcService
  ) {
    this.body = new Body();
  }

  ngOnInit() {
    this.canManageMep = Observable.of(false);
    this.route.parent.parent.params.forEach((params: Params) => {
      let id = params['id'];
      this.bodyId = id;
      this.canManageMep = this.oidcService.roles.map(roles => {
        return (roles.indexOf(Role.OrgaanBeheerder) !== -1 || roles.indexOf(Role.AlgemeenBeheerder) !== -1);
      });

      this.isLoading = true;
      this.subscriptions.push(this.bodyService
        .get(id)
        .subscribe(
          item => {
            if (item)
              this.body = item;
          },
          error => this.alertService.setAlert(
                new Alert(
                    AlertType.Error,
                    'Participaties kunnen niet geladen worden!',
                    'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));

      this.loadBodyParticipations(id);
      this.loadBodyParticipationTotals(id);
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<BodyParticipationReportFilter>) {
    this.filter = event.fields;
    this.loadBodyParticipations(this.bodyId);
    this.loadBodyParticipationTotals(this.bodyId);
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadBodyParticipations(this.bodyId, event);
  }

  get isNonCompliant() {
    return this.body.hasAllSeatsAssigned && this.bodyParticipationTotals.compliance === Compliance.NONCOMPLIANT;
  }

  private loadBodyParticipations(bodyId: string, event?: PagedEvent) {
    this.isLoading = true;
    let bodyParticipations = (event === undefined)
      ? this.bodyParticipationReportService.getParticipationsPerBody(bodyId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.bodyParticipationReportService.getParticipationsPerBody(bodyId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

      this.subscriptions.push(bodyParticipations
        .finally(() => this.isLoading = false)
        .subscribe(
          data => this.bodyParticipations = data,
          error => this.alertService.setAlert(
            new Alert(
              AlertType.Error,
              'Participaties kunnen niet geladen worden!',
              'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
  }

  private loadBodyParticipationTotals(bodyId: string) {
    this.isLoading = true;
    let bodyParticipationTotals = this.bodyParticipationReportService.getParticipationsPerBodyTotals(bodyId, this.filter);

    this.subscriptions.push(bodyParticipationTotals
      .finally(() => this.isLoading = false)
      .subscribe(
        data => this.bodyParticipationTotals = data,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Totalen voor participaties kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
  }
}
