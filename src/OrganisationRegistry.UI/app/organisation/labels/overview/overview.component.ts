import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationLabelListItem,
  OrganisationLabelService,
  OrganisationLabelFilter
} from 'services/organisationlabels';
import { OrganisationInfoService } from 'services';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationLabelsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationLabels: PagedResult<OrganisationLabelListItem>;

  private filter: OrganisationLabelFilter = new OrganisationLabelFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie benamingen');
  private organisationId: string;
  private currentSortBy: string = 'labelTypeName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationLabelService: OrganisationLabelService,
    private oidcService: OidcService,
    private alertService: AlertService,
    public store: OrganisationInfoService
  ) {
    this.organisationLabels = new PagedResult<OrganisationLabelListItem>();
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

  search(event: SearchEvent<OrganisationLabelFilter>) {
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
      ? this.organisationLabelService.getOrganisationLabels(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationLabelService.getOrganisationLabels(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    labels
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationLabels = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
