import {Component, OnDestroy, OnInit} from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  ContactType,
  ContactTypeService,
  ContactTypeFilter
} from 'services/contacttypes';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class ContactTypeOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public contactTypes: PagedResult<ContactType> = new PagedResult<ContactType>();

  private filter: ContactTypeFilter = new ContactTypeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private alertService: AlertService,
    private contactTypeService: ContactTypeService) { }

  ngOnInit() {
    this.loadContactTypes();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<ContactTypeFilter>) {
    this.filter = event.fields;
    this.loadContactTypes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadContactTypes(event);
  }

  private loadContactTypes(event?: PagedEvent) {
    this.isLoading = true;
    let contactTypes = (event === undefined)
      ? this.contactTypeService.getContactTypes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.contactTypeService.getContactTypes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(contactTypes
      .finally(() => this.isLoading = false)
      .subscribe(
        newContactTypes => this.contactTypes = newContactTypes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Contact types kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
  }
}
