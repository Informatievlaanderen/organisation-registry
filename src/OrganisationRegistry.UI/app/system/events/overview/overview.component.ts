import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  EventListItem,
  EventService,
  EventFilter
} from 'services/events';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class EventDataOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public events: PagedResult<EventListItem> = new PagedResult<EventListItem>();

  private filter: EventFilter = new EventFilter();
  private currentSortBy: string = 'number';
  private currentSortOrder: SortOrder = SortOrder.Descending;

  constructor(
    private alertService: AlertService,
    private eventService: EventService) { }

  ngOnInit() {
    this.loadEvents();
  }

  search(event: SearchEvent<EventFilter>) {
    this.filter = event.fields;
    this.loadEvents();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadEvents(event);
  }

  private loadEvents(event?: PagedEvent) {
    this.isLoading = true;
    let events = (event === undefined)
      ? this.eventService.getEvents(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.eventService.getEvents(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    events
      .finally(() => this.isLoading = false)
      .subscribe(
        newEvents => this.events = newEvents,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Events kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
