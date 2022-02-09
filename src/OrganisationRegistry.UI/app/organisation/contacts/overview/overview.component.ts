import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationContactListItem,
  OrganisationContactService,
  OrganisationContactFilter
} from 'services/organisationcontacts';
import { OrganisationInfoService } from 'services';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationContactsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationContacts: PagedResult<OrganisationContactListItem>;

  private filter: OrganisationContactFilter = new OrganisationContactFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie contacten');
  private organisationId: string;
  private currentSortBy: string = 'contactTypeName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationContactService: OrganisationContactService,
    private oidcService: OidcService,
    private alertService: AlertService,
    public store: OrganisationInfoService
  ) {
    this.organisationContacts = new PagedResult<OrganisationContactListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.loadContacts();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationContactFilter>) {
    this.filter = event.fields;
    this.loadContacts();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadContacts(event);
  }

  private loadContacts(event?: PagedEvent) {
    this.isLoading = true;
    let contacts = (event === undefined)
      ? this.organisationContactService.getOrganisationContacts(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationContactService.getOrganisationContacts(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(contacts
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationContacts = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build())));
  }
}
