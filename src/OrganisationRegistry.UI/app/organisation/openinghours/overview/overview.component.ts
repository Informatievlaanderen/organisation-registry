import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationOpeningHourListItem,
  OrganisationOpeningHourService,
  OrganisationOpeningHourFilter
} from 'services/organisationopeninghours';
import { OrganisationInfoService } from 'services';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationOpeningHoursOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationOpeningHours: PagedResult<OrganisationOpeningHourListItem>;

  private filter: OrganisationOpeningHourFilter = new OrganisationOpeningHourFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie benamingen');
  private organisationId: string;
  private currentSortBy: string = 'opens';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationOpeningHourService: OrganisationOpeningHourService,
    private oidcService: OidcService,
    private alertService: AlertService,
    public store: OrganisationInfoService
  ) {
    this.organisationOpeningHours = new PagedResult<OrganisationOpeningHourListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.loadLabels();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationOpeningHourFilter>) {
    this.filter = event.fields;
    this.loadLabels();
  }


  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadLabels(event);
  }

  private loadLabels(event?: PagedEvent) {
    this.isLoading = true;
    let labels = (event === undefined)
      ? this.organisationOpeningHourService.getOrganisationOpeningHours(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationOpeningHourService.getOrganisationOpeningHours(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    labels
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationOpeningHours = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
