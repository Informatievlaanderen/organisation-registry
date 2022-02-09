import {Component, OnDestroy, OnInit} from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  EventListItem,
  EventService,
  EventFilter
} from 'services/events';
import {BehaviorSubject} from "rxjs/BehaviorSubject";
import {Observable} from "rxjs/Observable";
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class EventDataOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public events: PagedResult<EventListItem> = new PagedResult<EventListItem>();
  public filterSource: BehaviorSubject<EventFilter>;
  public filterChanged: Observable<EventFilter>;

  private currentSortBy: string = 'number';
  private currentSortOrder: SortOrder = SortOrder.Descending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private alertService: AlertService,
    private eventService: EventService) {
    this.filterSource = new BehaviorSubject<EventFilter>(new EventFilter())
    this.filterChanged = this.filterSource.asObservable();
  }

  ngOnInit() {
    this.loadEvents();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<EventFilter>) {
    this.filterSource.next(event.fields);
    this.loadEvents();
  }

  setFilterAggregateId(aggregateId: string) {
    const value = this.filter;

    value.aggregateId = aggregateId;

    this.filterSource.next(value);
  }

  get filter() {
    return this.filterSource.getValue();
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

    this.subscriptions.push(events
      .finally(() => this.isLoading = false)
      .subscribe(
        newEvents => this.events = newEvents,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Events kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
  }
}
