import {Component, OnDestroy, OnInit} from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  LocationType,
  LocationTypeService,
  LocationTypeFilter
} from 'services/locationtypes';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class LocationTypeOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public locationTypes: PagedResult<LocationType> = new PagedResult<LocationType>();

  private filter: LocationTypeFilter = new LocationTypeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private alertService: AlertService,
    private locationTypeService: LocationTypeService) { }

  ngOnInit() {
    this.loadLocationTypes();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<LocationTypeFilter>) {
    this.filter = event.fields;
    this.loadLocationTypes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadLocationTypes(event);
  }

  private loadLocationTypes(event?: PagedEvent) {
    this.isLoading = true;
    let locationTypes = (event === undefined)
      ? this.locationTypeService.getLocationTypes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.locationTypeService.getLocationTypes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(locationTypes
      .finally(() => this.isLoading = false)
      .subscribe(
        newLocationTypes => this.locationTypes = newLocationTypes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Locatie types kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
  }
}
