import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  SeatTypeFilter,
  SeatTypeListItem,
  SeatType
} from './';

@Injectable()
export class SeatTypeService implements ICrudService<SeatType> {
  private seatTypesUrl = `${this.configurationService.apiUrl}/v1/seattypes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getSeatTypes(
    filter: SeatTypeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<SeatTypeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.seatTypesUrl, { headers: headers })
      .map(this.toSeatTypes);
  }

  public get(id: string): Observable<SeatType> {
    const url = `${this.seatTypesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toSeatType);
  }

  public search(filter: SeatTypeFilter): Observable<PagedResult<SeatTypeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.seatTypesUrl, { headers: headers })
      .map(this.toSeatTypes);
  }

  public getAllSeatTypes(): Observable<SeatTypeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.seatTypesUrl, { headers: headers })
      .map(this.toSeatTypes)
      .map(pagedResult => pagedResult.data);
  }

  public create(seatType: SeatType): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.seatTypesUrl, JSON.stringify(seatType), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(seatType: SeatType): Observable<string> {
    const url = `${this.seatTypesUrl}/${seatType.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(seatType), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toSeatType(res: Response): SeatType {
    let body = res.json();
    return body || {} as SeatType;
  }

  private toSeatTypes(res: Response): PagedResult<SeatTypeListItem> {
    return new PagedResultFactory<SeatTypeListItem>().create(res.headers, res.json());
  }
}
