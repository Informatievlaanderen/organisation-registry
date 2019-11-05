import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  LocationListItem,
  LocationService,
  LocationFilter
} from 'services/locations';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class LocationOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public locations: PagedResult<LocationListItem> = new PagedResult<LocationListItem>();

  private filter: LocationFilter = new LocationFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private locationService: LocationService) { }

  ngOnInit() {
    this.loadLocations();
  }

  search(event: SearchEvent<LocationFilter>) {
    this.filter = event.fields;
    this.loadLocations();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadLocations(event);
  }

  private loadLocations(event?: PagedEvent) {
    this.isLoading = true;
    let locations = (event === undefined)
      ? this.locationService.getLocations(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.locationService.getLocations(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    locations
      .finally(() => this.isLoading = false)
      .subscribe(
        newLocations => this.locations = newLocations,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Locaties kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'
          )));
  }
}
