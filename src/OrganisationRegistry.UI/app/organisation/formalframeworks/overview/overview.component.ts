import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationFormalFrameworkListItem,
  OrganisationFormalFrameworkService,
  OrganisationFormalFrameworkFilter
} from 'services/organisationformalframeworks';

import { OrganisationInfoService } from 'services/organisationinfo';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationFormalFrameworksOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationFormalFrameworks: PagedResult<OrganisationFormalFrameworkListItem>;

  private filter: OrganisationFormalFrameworkFilter = new OrganisationFormalFrameworkFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie historiek');
  private organisationId: string;
  private currentSortBy: string = 'parentOrganisationName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationFormalFrameworkService: OrganisationFormalFrameworkService,
    private oidcService: OidcService,
    private alertService: AlertService,
    public store: OrganisationInfoService
  ) {
    this.organisationFormalFrameworks = new PagedResult<OrganisationFormalFrameworkListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.loadFormalFrameworks();
      this.store.loadOrganisation(this.organisationId);
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationFormalFrameworkFilter>) {
    this.filter = event.fields;
    this.loadFormalFrameworks();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadFormalFrameworks(event);
  }

  private loadFormalFrameworks(event?: PagedEvent) {
    this.isLoading = true;
    let organisationFormalFrameworks = (event === undefined)
      ? this.organisationFormalFrameworkService.getOrganisationFormalFrameworks(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationFormalFrameworkService.getOrganisationFormalFrameworks(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(organisationFormalFrameworks
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationFormalFrameworks = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build())));
  }
}
