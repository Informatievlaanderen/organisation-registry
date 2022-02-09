import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';

import {
  OrganisationParentListItem,
  OrganisationParentService
} from 'services/organisationparents';

import { OrganisationInfoService } from 'services/organisationinfo';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationParentsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationParents: PagedResult<OrganisationParentListItem>;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie historiek');
  private organisationId: string;
  private currentSortBy: string = 'parentOrganisationName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationParentService: OrganisationParentService,
    private oidcService: OidcService,
    private alertService: AlertService,
    public store: OrganisationInfoService
  ) {
    this.organisationParents = new PagedResult<OrganisationParentListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.loadParents();
      this.store.loadOrganisation(this.organisationId);
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadParents(event);
  }

  private loadParents(event?: PagedEvent) {
    this.isLoading = true;
    let organisationParents = (event === undefined)
      ? this.organisationParentService.getOrganisationParents(this.organisationId, this.currentSortBy, this.currentSortOrder)
      : this.organisationParentService.getOrganisationParents(this.organisationId, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(organisationParents
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationParents = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build())));
  }
}
