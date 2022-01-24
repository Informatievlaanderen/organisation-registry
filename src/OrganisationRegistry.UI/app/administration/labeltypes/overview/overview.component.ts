import {Component, OnDestroy, OnInit} from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  LabelType,
  LabelTypeService,
  LabelTypeFilter
} from 'services/labeltypes';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class LabelTypeOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public labelTypes: PagedResult<LabelType> = new PagedResult<LabelType>();

  private filter: LabelTypeFilter = new LabelTypeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private alertService: AlertService,
    private labelTypeService: LabelTypeService) { }

  ngOnInit() {
    this.loadLabelTypes();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<LabelTypeFilter>) {
    this.filter = event.fields;
    this.loadLabelTypes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadLabelTypes(event);
  }

  private loadLabelTypes(event?: PagedEvent) {
    this.isLoading = true;
    let labelTypes = (event === undefined)
      ? this.labelTypeService.getLabelTypes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.labelTypeService.getLabelTypes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(labelTypes
      .finally(() => this.isLoading = false)
      .subscribe(
        newLabelTypes => this.labelTypes = newLabelTypes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Benaming types kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
  }
}
