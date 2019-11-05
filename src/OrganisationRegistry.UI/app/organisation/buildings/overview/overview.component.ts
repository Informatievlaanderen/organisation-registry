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
  OrganisationBuildingListItem,
  OrganisationBuildingService,
  OrganisationBuildingFilter
} from 'services/organisationbuildings';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationBuildingsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationBuildings: PagedResult<OrganisationBuildingListItem>;
  public canEditOrganisation: Observable<boolean>;

  private filter: OrganisationBuildingFilter = new OrganisationBuildingFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie gebouwen');
  private organisationId: string;
  private currentSortBy: string = 'buildingName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationBuildingService: OrganisationBuildingService,
    private oidcService: OidcService,
    private alertService: AlertService
  ) {
    this.organisationBuildings = new PagedResult<OrganisationBuildingListItem>();
  }

  ngOnInit() {
    this.canEditOrganisation = Observable.of(false);
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.canEditOrganisation = this.oidcService.canEditOrganisation(this.organisationId);
      this.loadBuildings();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationBuildingFilter>) {
    this.filter = event.fields;
    this.loadBuildings();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadBuildings(event);
  }

  private loadBuildings(event?: PagedEvent) {
    this.isLoading = true;
    let buildings = (event === undefined)
      ? this.organisationBuildingService.getOrganisationBuildings(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationBuildingService.getOrganisationBuildings(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    buildings
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationBuildings = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
