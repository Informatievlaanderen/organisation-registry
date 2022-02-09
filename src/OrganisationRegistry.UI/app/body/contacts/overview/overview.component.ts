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
  BodyContactListItem,
  BodyContactService,
  BodyContactFilter
} from 'services/bodycontacts';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class BodyContactsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public bodyContacts: PagedResult<BodyContactListItem>;
  public canEditBody: Observable<boolean>;

  private filter: BodyContactFilter = new BodyContactFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Orgaan contacten');
  private bodyId: string;
  private currentSortBy: string = 'contactTypeName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bodyContactService: BodyContactService,
    private oidcService: OidcService,
    private alertService: AlertService
  ) {
    this.bodyContacts = new PagedResult<BodyContactListItem>();
  }

  ngOnInit() {
    this.canEditBody = Observable.of(false);
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.bodyId = params['id'];
      this.canEditBody = this.oidcService.canEditBody(this.bodyId);
      this.loadContacts();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<BodyContactFilter>) {
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
      ? this.bodyContactService.getBodyContacts(this.bodyId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.bodyContactService.getBodyContacts(this.bodyId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(contacts
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.bodyContacts = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build())));
  }
}
