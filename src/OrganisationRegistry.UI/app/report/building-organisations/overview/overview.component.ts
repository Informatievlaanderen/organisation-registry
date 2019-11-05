import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  Building,
  BuildingService,
  BuildingFilter
} from 'services/buildings';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class BuildingOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public buildings: PagedResult<Building> = new PagedResult<Building>();

  private filter: BuildingFilter = new BuildingFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private buildingService: BuildingService) { }

  ngOnInit() {
    this.loadBuildings();
  }

  search(event: SearchEvent<BuildingFilter>) {
    this.filter = event.fields;
    this.loadBuildings();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadBuildings(event);
  }

  private loadBuildings(event?: PagedEvent) {
    this.isLoading = true;
    let buildings = (event === undefined)
      ? this.buildingService.getBuildings(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.buildingService.getBuildings(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    buildings
      .finally(() => this.isLoading = false)
      .subscribe(
        newBuildings => this.buildings = newBuildings,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Gebouwen kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
