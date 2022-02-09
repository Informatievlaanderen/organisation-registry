import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';

import {
  BodyLifecyclePhaseListItem,
  BodyLifecyclePhaseService
} from 'services/bodylifecyclephases';

import { BodyInfoService } from 'services/bodyinfo';
import { Body } from 'services/bodies';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class BodyLifecyclePhasesOverviewComponent implements OnInit, OnDestroy {
  public body: Body;

  public isLoading: boolean = true;
  public bodyLifecyclePhases: PagedResult<BodyLifecyclePhaseListItem>;
  public canEditBody: Observable<boolean>;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Levensloopfasen');
  private bodyId: string;
  private currentSortBy: string = 'validFrom';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bodyLifecyclePhaseService: BodyLifecyclePhaseService,
    private oidcService: OidcService,
    private alertService: AlertService,
    private store: BodyInfoService
  ) {
    this.body = new Body();
    this.bodyLifecyclePhases = new PagedResult<BodyLifecyclePhaseListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.store
      .bodyChanged
      .subscribe(body => this.body = body));

    this.canEditBody = Observable.of(false);
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.bodyId = params['id'];
      this.canEditBody = this.oidcService.canEditBody(this.bodyId);
      this.loadBodyLifecyclePhases();
      this.store.loadBody(this.bodyId);
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadBodyLifecyclePhases(event);
  }

  private loadBodyLifecyclePhases(event?: PagedEvent) {
    this.isLoading = true;
    let bodyLifecyclePhases = (event === undefined)
      ? this.bodyLifecyclePhaseService.getBodyLifecyclePhases(this.bodyId, this.currentSortBy, this.currentSortOrder)
      : this.bodyLifecyclePhaseService.getBodyLifecyclePhases(this.bodyId, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(bodyLifecyclePhases
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.bodyLifecyclePhases = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build())));
  }
}
