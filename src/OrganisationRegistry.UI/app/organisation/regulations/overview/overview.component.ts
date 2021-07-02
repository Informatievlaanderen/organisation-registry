import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationRegulationListItem,
  OrganisationRegulationService,
  OrganisationRegulationFilter
} from 'services/organisationregulations';
import { OrganisationInfoService } from 'services';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationRegulationsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationRegulations: PagedResult<OrganisationRegulationListItem>;

  private filter: OrganisationRegulationFilter = new OrganisationRegulationFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie regulationen');
  private organisationId: string;
  private currentSortBy: string = 'regulationTypeName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationRegulationService: OrganisationRegulationService,
    private oidcService: OidcService,
    private alertService: AlertService,
    public store: OrganisationInfoService
  ) {
    this.organisationRegulations = new PagedResult<OrganisationRegulationListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.loadRegulations();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationRegulationFilter>) {
    this.filter = event.fields;
    this.loadRegulations();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadRegulations(event);
  }

  private loadRegulations(event?: PagedEvent) {
    this.isLoading = true;
    let regulations = (event === undefined)
      ? this.organisationRegulationService.getOrganisationRegulations(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationRegulationService.getOrganisationRegulations(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    regulations
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationRegulations = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
