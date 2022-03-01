import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationBodyListItem,
  OrganisationBodyService,
  OrganisationBodyFilter
} from 'services/organisationbodies';
import {OrganisationInfoService} from "services";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationBodiesOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationId: string;
  public organisationBodies: PagedResult<OrganisationBodyListItem>;

  private filter: OrganisationBodyFilter = new OrganisationBodyFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie organen');
  private currentSortBy: string = 'bodyName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationBodyService: OrganisationBodyService,
    private oidcService: OidcService,
    private alertService: AlertService,
    private store: OrganisationInfoService
  ) {
    this.organisationBodies = new PagedResult<OrganisationBodyListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.loadBodies();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationBodyFilter>) {
    this.filter = event.fields;
    this.loadBodies();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadBodies(event);
  }

  private loadBodies(event?: PagedEvent) {
    this.isLoading = true;
    let bodies = (event === undefined)
      ? this.organisationBodyService.getOrganisationBodies(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationBodyService.getOrganisationBodies(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(bodies
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationBodies = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build())));
  }
}
