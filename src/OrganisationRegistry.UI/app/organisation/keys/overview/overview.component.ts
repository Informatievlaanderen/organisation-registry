import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Params, Router} from '@angular/router';

import {Observable} from 'rxjs/Observable';
import {Subscription} from 'rxjs/Subscription';

import {OidcService} from 'core/auth';
import {AlertBuilder, AlertService} from 'core/alert';
import {BaseAlertMessages} from 'core/alertmessages';
import {PagedEvent, PagedResult, SortOrder} from 'core/pagination';
import {SearchEvent} from 'core/search';

import {OrganisationKeyFilter, OrganisationKeyListItem, OrganisationKeyService} from 'services/organisationkeys';
import {OrganisationInfoService} from "../../../services";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationKeysOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationKeys: PagedResult<OrganisationKeyListItem>;

  private filter: OrganisationKeyFilter = new OrganisationKeyFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie sleutels');
  private organisationId: string;
  private currentSortBy: string = 'keyTypeName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationKeyService: OrganisationKeyService,
    private oidcService: OidcService,
    private alertService: AlertService,
    private store: OrganisationInfoService
  ) {
    this.organisationKeys = new PagedResult<OrganisationKeyListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.store
      .organisationChanged
      .subscribe(org => {
      }));

    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.loadKeys();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationKeyFilter>) {
    this.filter = event.fields;
    this.loadKeys();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadKeys(event);
  }

  private loadKeys(event?: PagedEvent) {
    this.isLoading = true;
    let keys = (event === undefined)
      ? this.organisationKeyService.getOrganisationKeys(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationKeyService.getOrganisationKeys(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(keys
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationKeys = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build())));
  }
}
