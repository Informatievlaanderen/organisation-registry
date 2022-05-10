import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  CapacityFilter,
  CapacityListItem,
  Capacity
} from './';
import {KeyType} from "../keytypes";

@Injectable()
export class CapacityService implements ICrudService<Capacity> {
  private capacitiesUrl = `${this.configurationService.apiUrl}/v1/capacities`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getCapacities(
    filter: CapacityFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<CapacityListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.capacitiesUrl, { headers: headers })
      .map(this.toCapacities);
  }

  public get(id: string): Observable<Capacity> {
    const url = `${this.capacitiesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toCapacity);
  }

  public search(filter: CapacityFilter): Observable<PagedResult<CapacityListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.capacitiesUrl, { headers: headers })
      .map(this.toCapacities);
  }

  public getAllCapacities(): Observable<CapacityListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.capacitiesUrl, { headers: headers })
      .map(this.toCapacities)
      .map(pagedResult => pagedResult.data);
  }

  public create(capacity: Capacity): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.capacitiesUrl, JSON.stringify(capacity), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(capacity: Capacity): Observable<string> {
    const url = `${this.capacitiesUrl}/${capacity.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(capacity), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toCapacity(res: Response): Capacity {
    let body = res.json();
    return body || {} as Capacity;
  }

  private toCapacities(res: Response): PagedResult<CapacityListItem> {
    return new PagedResultFactory<CapacityListItem>().create(res.headers, res.json());
  }

  delete(capacity: Capacity) {
    const url = `${this.capacitiesUrl}/${capacity.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .delete(url, { headers: headers });
  }
}
