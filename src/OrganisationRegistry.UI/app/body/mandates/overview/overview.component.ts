import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { AuthService, OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { SearchEvent } from 'core/search';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';

import {
  BodySeatService
} from 'services/bodyseats';

import {
  BodyMandateListItem,
  BodyMandateService,
  BodyMandateFilter
} from 'services/bodymandates';

import { BodyInfoService } from 'services/bodyinfo';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class BodyMandatesOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public bodyMandates: PagedResult<BodyMandateListItem>;
  public canEditBody: Observable<boolean>;
  public hasSeats: Observable<boolean>;

  private filter: BodyMandateFilter = new BodyMandateFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Mandaten');
  private bodyId: string;
  private currentSortBy: string = 'bodySeatTypeOrder';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bodySeatService: BodySeatService,
    private bodyMandateService: BodyMandateService,
    private oidcService: OidcService,
    private alertService: AlertService,
    private store: BodyInfoService
  ) {
    this.bodyMandates = new PagedResult<BodyMandateListItem>();
  }

  ngOnInit() {
    this.canEditBody = Observable.of(false);
    this.hasSeats = Observable.of(false);

    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.bodyId = params['id'];
      this.canEditBody = this.oidcService.canEditBody(this.bodyId);
      this.loadMandates();
      this.store.loadBody(this.bodyId);

      this.hasSeats = this.bodySeatService.hasBodySeats(this.bodyId);
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<BodyMandateFilter>) {
    this.filter = event.fields;
    this.loadMandates();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadMandates(event);
  }

  private loadMandates(event?: PagedEvent) {
    this.isLoading = true;
    let bodyMandates = (event === undefined)
      ? this.bodyMandateService.getBodyMandates(this.bodyId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.bodyMandateService.getBodyMandates(this.bodyId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    bodyMandates
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.bodyMandates = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
