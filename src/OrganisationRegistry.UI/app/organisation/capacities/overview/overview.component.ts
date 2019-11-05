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
  OrganisationCapacityListItem,
  OrganisationCapacityService,
  OrganisationCapacityFilter
} from 'services/organisationcapacities';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationCapacitiesOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationCapacities: PagedResult<OrganisationCapacityListItem>;
  public canEditOrganisation: Observable<boolean>;

  private filter: OrganisationCapacityFilter = new OrganisationCapacityFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie hoedanigheden');
  private organisationId: string;
  private currentSortBy: string = 'capacityName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationCapacityService: OrganisationCapacityService,
    private oidcService: OidcService,
    private alertService: AlertService
  ) {
    this.organisationCapacities = new PagedResult<OrganisationCapacityListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.canEditOrganisation = this.oidcService.canEditOrganisation(this.organisationId);
      this.loadCapacities();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationCapacityFilter>) {
    this.filter = event.fields;
    this.loadCapacities();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadCapacities(event);
  }

  private loadCapacities(event?: PagedEvent) {
    this.isLoading = true;
    let organisationCapacities = (event === undefined)
      ? this.organisationCapacityService.getOrganisationCapacities(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationCapacityService.getOrganisationCapacities(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    organisationCapacities
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationCapacities = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
