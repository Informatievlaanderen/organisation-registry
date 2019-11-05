import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  LifecyclePhaseType,
  LifecyclePhaseTypeService,
  LifecyclePhaseTypeFilter
} from 'services/lifecyclephasetypes';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class LifecyclePhaseTypeOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public lifecyclePhaseTypes: PagedResult<LifecyclePhaseType> = new PagedResult<LifecyclePhaseType>();

  private filter: LifecyclePhaseTypeFilter = new LifecyclePhaseTypeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private lifecyclePhaseTypeService: LifecyclePhaseTypeService) { }

  ngOnInit() {
    this.loadLifecyclePhaseTypes();
  }

  search(event: SearchEvent<LifecyclePhaseTypeFilter>) {
    this.filter = event.fields;
    this.loadLifecyclePhaseTypes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadLifecyclePhaseTypes(event);
  }

  private loadLifecyclePhaseTypes(event?: PagedEvent) {
    this.isLoading = true;
    let lifecyclePhaseTypes = (event === undefined)
      ? this.lifecyclePhaseTypeService.getLifecyclePhaseTypes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.lifecyclePhaseTypeService.getLifecyclePhaseTypes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    lifecyclePhaseTypes
      .finally(() => this.isLoading = false)
      .subscribe(
        newLifecyclePhaseTypes => this.lifecyclePhaseTypes = newLifecyclePhaseTypes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Levensloopfase types kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
