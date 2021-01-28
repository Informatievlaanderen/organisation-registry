import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationRelationListItem,
  OrganisationRelationService,
  OrganisationRelationFilter
} from 'services/organisationrelations';
import {OrganisationInfoService} from "services";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationRelationsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationRelations: PagedResult<OrganisationRelationListItem>;

  private filter: OrganisationRelationFilter = new OrganisationRelationFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie relaties');
  private organisationId: string;
  private currentSortBy: string = 'relationName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationRelationService: OrganisationRelationService,
    private oidcService: OidcService,
    private alertService: AlertService,
    public store: OrganisationInfoService
  ) {
    this.organisationRelations = new PagedResult<OrganisationRelationListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.loadRelations();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationRelationFilter>) {
    this.filter = event.fields;
    this.loadRelations();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadRelations(event);
  }

  private loadRelations(event?: PagedEvent) {
    this.isLoading = true;
    let organisationRelations = (event === undefined)
      ? this.organisationRelationService.getOrganisationRelations(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationRelationService.getOrganisationRelations(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    organisationRelations
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationRelations = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
