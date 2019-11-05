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
  BodySeatListItem,
  BodySeatService,
  BodySeatFilter
} from 'services/bodyseats';

import { BodyInfoService } from 'services/bodyinfo';
import { Body } from 'services/bodies';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class BodySeatsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public bodySeats: PagedResult<BodySeatListItem>;
  public canEditBody: Observable<boolean>;

  private filter: BodySeatFilter = new BodySeatFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Posten');
  private bodyId: string;
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bodySeatService: BodySeatService,
    private oidcService: OidcService,
    private alertService: AlertService
  ) {
    this.bodySeats = new PagedResult<BodySeatListItem>();
  }

  ngOnInit() {
    this.canEditBody = Observable.of(false);
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.bodyId = params['id'];
      this.canEditBody = this.oidcService.canEditBody(this.bodyId);
      this.loadSeats();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<BodySeatFilter>) {
    this.filter = event.fields;
    this.loadSeats();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadSeats(event);
  }

  private loadSeats(event?: PagedEvent) {
    this.isLoading = true;
    let bodySeats = (event === undefined)
      ? this.bodySeatService.getBodySeats(this.bodyId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.bodySeatService.getBodySeats(this.bodyId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    bodySeats
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.bodySeats = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
