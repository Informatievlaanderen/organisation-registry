import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  PurposeFilter,
  PurposeListItem,
  Purpose
} from './';

@Injectable()
export class PurposeService implements ICrudService<Purpose> {
  private purposesUrl = `${this.configurationService.apiUrl}/v1/purposes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getPurposes(
    filter: PurposeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<PurposeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.purposesUrl, { headers: headers })
      .map(this.toPurposes);
  }

  public get(id: string): Observable<Purpose> {
    const url = `${this.purposesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toPurpose);
  }

  public search(filter: PurposeFilter): Observable<PagedResult<PurposeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.purposesUrl, { headers: headers })
      .map(this.toPurposes);
  }

  public getAllPurposes(): Observable<PurposeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.purposesUrl, { headers: headers })
      .map(this.toPurposes)
      .map(pagedResult => pagedResult.data);
  }

  public create(aPurpose: Purpose): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.purposesUrl, JSON.stringify(aPurpose), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(aPurpose: Purpose): Observable<string> {
    const url = `${this.purposesUrl}/${aPurpose.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(aPurpose), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toPurpose(res: Response): Purpose {
    let body = res.json();
    return body || {} as Purpose;
  }

  private toPurposes(res: Response): PagedResult<PurposeListItem> {
    return new PagedResultFactory<PurposeListItem>().create(res.headers, res.json());
  }
}
