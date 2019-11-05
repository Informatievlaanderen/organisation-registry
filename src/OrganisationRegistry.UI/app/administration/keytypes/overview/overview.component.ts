import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  KeyTypeListItem,
  KeyTypeService,
  KeyTypeFilter
} from 'services/keytypes';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class KeyTypeOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public keyTypes: PagedResult<KeyTypeListItem> = new PagedResult<KeyTypeListItem>();

  private filter: KeyTypeFilter = new KeyTypeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private keyTypeService: KeyTypeService) { }

  ngOnInit() {
    this.loadKeyTypes();
  }

  search(event: SearchEvent<KeyTypeFilter>) {
    this.filter = event.fields;
    this.loadKeyTypes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadKeyTypes(event);
  }

  private loadKeyTypes(event?: PagedEvent) {
    this.isLoading = true;
    let keyTypes = (event === undefined)
      ? this.keyTypeService.getKeyTypes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.keyTypeService.getKeyTypes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    keyTypes
      .finally(() => this.isLoading = false)
      .subscribe(
        newKeyTypes => this.keyTypes = newKeyTypes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Informatiesystemen kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'
          )));
  }
}
