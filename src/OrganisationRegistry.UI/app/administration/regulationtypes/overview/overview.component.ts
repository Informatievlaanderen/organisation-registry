import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  RegulationType,
  RegulationTypeService,
  RegulationTypeFilter
} from 'services/regulationtypes';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class RegulationTypeOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public regulationTypes: PagedResult<RegulationType> = new PagedResult<RegulationType>();

  private filter: RegulationTypeFilter = new RegulationTypeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private regulationTypeService: RegulationTypeService) { }

  ngOnInit() {
    this.loadRegulationTypes();
  }

  search(event: SearchEvent<RegulationTypeFilter>) {
    this.filter = event.fields;
    this.loadRegulationTypes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadRegulationTypes(event);
  }

  private loadRegulationTypes(event?: PagedEvent) {
    this.isLoading = true;
    let regulationTypes = (event === undefined)
      ? this.regulationTypeService.getRegulationTypes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.regulationTypeService.getRegulationTypes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    regulationTypes
      .finally(() => this.isLoading = false)
      .subscribe(
        newRegulationTypes => this.regulationTypes = newRegulationTypes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Regelgevingstypes kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
