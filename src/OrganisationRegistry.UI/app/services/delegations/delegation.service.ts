import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  DelegationFilter,
  DelegationListItem,
  Delegation
} from './';

@Injectable()
export class DelegationService implements ICrudService<Delegation> {
  private delegationsUrl = `${this.configurationService.apiUrl}/v1/manage/delegations`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getDelegations(
    filter: DelegationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<DelegationListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.delegationsUrl, { headers: headers })
      .map(this.toDelegations);
  }

  public get(id: string): Observable<Delegation> {
    const url = `${this.delegationsUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toDelegation);
  }

  public search(filter: DelegationFilter): Observable<PagedResult<DelegationListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.delegationsUrl, { headers: headers })
      .map(this.toDelegations);
  }

  public hasNonDelegatedDelegations(filter: DelegationFilter): Observable<boolean> {
    var newFilter = JSON.parse(JSON.stringify(filter));
    newFilter.emptyDelegationsOnly = true;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(newFilter)
      .withPagination(1, 1)
      .build();

    return this.http
      .get(this.delegationsUrl, { headers: headers })
      .map(this.toDelegations)
      .map(x => x.totalItems > 0);
  }

  public getAllDelegations(): Observable<DelegationListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('bodyName', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.delegationsUrl, { headers: headers })
      .map(this.toDelegations)
      .map(pagedResult => pagedResult.data);
  }

  public create(aDelegation: Delegation): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.delegationsUrl, JSON.stringify(aDelegation), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(aDelegation: Delegation): Observable<string> {
    const url = `${this.delegationsUrl}/${aDelegation.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(aDelegation), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toDelegation(res: Response): Delegation {
    let body = res.json();
    return body || {} as Delegation;
  }

  private toDelegations(res: Response): PagedResult<DelegationListItem> {
    return new PagedResultFactory<DelegationListItem>().create(res.headers, res.json());
  }
}
