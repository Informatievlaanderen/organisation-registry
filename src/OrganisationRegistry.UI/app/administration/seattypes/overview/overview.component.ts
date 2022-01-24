﻿import {Component, OnDestroy, OnInit} from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  SeatType,
  SeatTypeService,
  SeatTypeFilter
} from 'services/seattypes';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class SeatTypeOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public seatTypes: PagedResult<SeatType> = new PagedResult<SeatType>();

  private filter: SeatTypeFilter = new SeatTypeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private alertService: AlertService,
    private seatTypeService: SeatTypeService) { }

  ngOnInit() {
    this.loadSeatTypes();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<SeatTypeFilter>) {
    this.filter = event.fields;
    this.loadSeatTypes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadSeatTypes(event);
  }

  private loadSeatTypes(event?: PagedEvent) {
    this.isLoading = true;
    let seatTypes = (event === undefined)
      ? this.seatTypeService.getSeatTypes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.seatTypeService.getSeatTypes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(seatTypes
      .finally(() => this.isLoading = false)
      .subscribe(
        newSeatTypes => this.seatTypes = newSeatTypes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Post types kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
  }
}
