import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { BodySeatListItem } from './body-seat-list-item.model';
import { BodySeat } from './body-seat.model';
import { BodySeatFilter } from './body-seat-filter.model';

import { CreateBodySeatRequest, UpdateBodySeatRequest } from './';

@Injectable()
export class BodySeatService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getBodySeats(
    bodyId: string,
    filter: BodySeatFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BodySeatListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getBodySeatsUrl(bodyId), { headers: headers })
      .map(this.toBodySeats);
  }

  public get(bodyId: string, bodySeatId: string): Observable<BodySeat> {
    const url = `${this.getBodySeatsUrl(bodyId)}/${bodySeatId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodySeat);
  }

  public getAllBodySeats(bodyId: string): Observable<BodySeatListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.getBodySeatsUrl(bodyId), { headers: headers })
      .map(this.toBodySeats)
      .map(pagedResult => pagedResult.data);
  }

  public hasBodySeats(bodyId: string): Observable<boolean> {
    let headers = new HeadersBuilder()
      .json()
      .withPagination(1, 1)
      .build();

    return this.http
      .get(this.getBodySeatsUrl(bodyId), { headers: headers })
      .map(this.toBodySeats)
      .map(x => x.totalItems > 0);
  }

  public create(bodyId, bodySeat: CreateBodySeatRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getBodySeatsUrl(bodyId), JSON.stringify(bodySeat), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(bodyId, bodySeat: UpdateBodySeatRequest): Observable<boolean> {
    const url = `${this.getBodySeatsUrl(bodyId)}/${bodySeat.bodySeatId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(bodySeat), { headers: headers })
      .map(response => response.ok);
  }

  private getBodySeatsUrl(bodyId) {
    return `${this.configurationService.apiUrl}/v1/bodies/${bodyId}/seats`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toBodySeat(res: Response): BodySeat {
    let body = res.json();
    return body || {} as BodySeat;
  }

  private toBodySeats(res: Response): PagedResult<BodySeatListItem> {
    return new PagedResultFactory<BodySeatListItem>().create(res.headers, res.json());
  }
}
