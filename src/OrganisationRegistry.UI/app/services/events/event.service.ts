import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  EventFilter,
  EventListItem,
  EventData
} from './';

@Injectable()
export class EventService implements ICrudService<EventData> {
  private eventsUrl = `${this.configurationService.apiUrl}/v1/events`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getEvents(
    filter: EventFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<EventListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.eventsUrl, { headers: headers })
      .map(this.toEvents);
  }

  public get(id: string): Observable<EventData> {
    const url = `${this.eventsUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toEvent);
  }

  public search(filter: EventFilter): Observable<PagedResult<EventListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.eventsUrl, { headers: headers })
      .map(this.toEvents);
  }

  public getAllEvents(): Observable<EventListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('number', SortOrder.Descending)
      .build();

    return this.http
      .get(this.eventsUrl, { headers: headers })
      .map(this.toEvents)
      .map(pagedResult => pagedResult.data);
  }

  public create(eventData: EventData): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.eventsUrl, JSON.stringify(eventData), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(eventData: EventData): Observable<string> {
    const url = `${this.eventsUrl}/${eventData.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(eventData), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toEvent(res: Response): EventData {
    let body = res.json();
    return body || {} as EventData;
  }

  private toEvents(res: Response): PagedResult<EventListItem> {
    return new PagedResultFactory<EventListItem>().create(res.headers, res.json());
  }
}
