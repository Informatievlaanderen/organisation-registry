import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationFunctionListItem,
  OrganisationFunctionService,
  OrganisationFunctionFilter
} from 'services/organisationfunctions';
import { OrganisationInfoService } from 'services';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationFunctionsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationFunctions: PagedResult<OrganisationFunctionListItem>;

  private filter: OrganisationFunctionFilter = new OrganisationFunctionFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie functies');
  private organisationId: string;
  private currentSortBy: string = 'personName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationFunctionService: OrganisationFunctionService,
    private oidcService: OidcService,
    private alertService: AlertService,
    public store: OrganisationInfoService
  ) {
    this.organisationFunctions = new PagedResult<OrganisationFunctionListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.loadFunctions();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationFunctionFilter>) {
    this.filter = event.fields;
    this.loadFunctions();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadFunctions(event);
  }

  private loadFunctions(event?: PagedEvent) {
    this.isLoading = true;
    let organisationFunctions = (event === undefined)
      ? this.organisationFunctionService.getOrganisationFunctions(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationFunctionService.getOrganisationFunctions(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(organisationFunctions
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationFunctions = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build())));
  }
}
