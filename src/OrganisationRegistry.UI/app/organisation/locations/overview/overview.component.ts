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
  OrganisationLocationListItem,
  OrganisationLocationService,
  OrganisationLocationFilter
} from 'services/organisationlocations';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationLocationsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationLocations: PagedResult<OrganisationLocationListItem>;
  public canEditOrganisation: Observable<boolean>;

  private filter: OrganisationLocationFilter = new OrganisationLocationFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie locaties');
  private organisationId: string;
  private currentSortBy: string = 'locationName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationLocationService: OrganisationLocationService,
    private oidcService: OidcService,
    private alertService: AlertService
  ) {
    this.organisationLocations = new PagedResult<OrganisationLocationListItem>();
  }

  ngOnInit() {
    this.canEditOrganisation = Observable.of(false);
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.canEditOrganisation = this.oidcService.canEditOrganisation(this.organisationId);
      this.loadLocations();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationLocationFilter>) {
    this.filter = event.fields;
    this.loadLocations();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadLocations(event);
  }

  private loadLocations(event?: PagedEvent) {
    this.isLoading = true;
    let locations = (event === undefined)
      ? this.organisationLocationService.getOrganisationLocations(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationLocationService.getOrganisationLocations(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    locations
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationLocations = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
