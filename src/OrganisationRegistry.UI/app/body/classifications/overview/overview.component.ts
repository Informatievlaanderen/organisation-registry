import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  BodyBodyClassificationListItem,
  BodyBodyClassificationService,
  BodyBodyClassificationFilter
} from 'services/bodybodyclassifications';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class BodyBodyClassificationsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public bodyBodyClassifications: PagedResult<BodyBodyClassificationListItem>;
  public canEditBody: Observable<boolean>;

  private filter: BodyBodyClassificationFilter = new BodyBodyClassificationFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Orgaan classificaties');
  private bodyId: string;
  private currentSortBy: string = 'bodyClassificationTypeName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bodyBodyClassificationService: BodyBodyClassificationService,
    private oidcService: OidcService,
    private alertService: AlertService
  ) {
    this.bodyBodyClassifications = new PagedResult<BodyBodyClassificationListItem>();
  }

  ngOnInit() {
    this.canEditBody = Observable.of(false);
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.bodyId = params['id'];
      this.canEditBody = this.oidcService.canEditBody(this.bodyId);
      this.loadBodyClassifications();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<BodyBodyClassificationFilter>) {
    this.filter = event.fields;
    this.loadBodyClassifications();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadBodyClassifications(event);
  }

  private loadBodyClassifications(event?: PagedEvent) {
    this.isLoading = true;
    let bodyClassifications = (event === undefined)
      ? this.bodyBodyClassificationService.getBodyBodyClassifications(this.bodyId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.bodyBodyClassificationService.getBodyBodyClassifications(this.bodyId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(bodyClassifications
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.bodyBodyClassifications = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build())));
  }
}
