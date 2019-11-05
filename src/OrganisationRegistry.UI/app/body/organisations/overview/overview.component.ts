import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { AuthService, OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  BodyOrganisationListItem,
  BodyOrganisationService,
  BodyOrganisationFilter
} from 'services/bodyorganisations';

import { BodyInfoService } from 'services/bodyinfo';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class BodyOrganisationsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public bodyOrganisations: PagedResult<BodyOrganisationListItem>;
  public canEditBody: Observable<boolean>;

  private filter: BodyOrganisationFilter = new BodyOrganisationFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisaties');
  private bodyId: string;
  private currentSortBy: string = 'organisationName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bodyOrganisationService: BodyOrganisationService,
    private oidcService: OidcService,
    private alertService: AlertService,
    private store: BodyInfoService
  ) {
    this.bodyOrganisations = new PagedResult<BodyOrganisationListItem>();
  }

  ngOnInit() {
    this.canEditBody = Observable.of(false);
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.bodyId = params['id'];
      this.canEditBody = this.oidcService.canEditBody(this.bodyId);
      this.loadOrganisations();
      this.store.loadBody(this.bodyId);
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<BodyOrganisationFilter>) {
    this.filter = event.fields;
    this.loadOrganisations();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadOrganisations(event);
  }

  private loadOrganisations(event?: PagedEvent) {
    this.isLoading = true;
    let bodyOrganisations = (event === undefined)
      ? this.bodyOrganisationService.getBodyOrganisations(this.bodyId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.bodyOrganisationService.getBodyOrganisations(this.bodyId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    bodyOrganisations
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.bodyOrganisations = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
