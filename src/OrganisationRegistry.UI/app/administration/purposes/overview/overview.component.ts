import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  Purpose,
  PurposeService,
  PurposeFilter
} from 'services/purposes';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class PurposeOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public purposes: PagedResult<Purpose> = new PagedResult<Purpose>();

  private filter: PurposeFilter = new PurposeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private purposeService: PurposeService) { }

  ngOnInit() {
    this.loadPurposes();
  }

  search(event: SearchEvent<PurposeFilter>) {
    this.filter = event.fields;
    this.loadPurposes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadPurposes(event);
  }

  private loadPurposes(event?: PagedEvent) {
    this.isLoading = true;
    let purposes = (event === undefined)
      ? this.purposeService.getPurposes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.purposeService.getPurposes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    purposes
      .finally(() => this.isLoading = false)
      .subscribe(
        newPurposes => this.purposes = newPurposes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Beleidsvelden kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
