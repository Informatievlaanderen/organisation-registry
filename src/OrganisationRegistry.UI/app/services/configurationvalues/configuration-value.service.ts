import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  ConfigurationValueFilter,
  ConfigurationValueListItem,
  ConfigurationValue
} from './';

@Injectable()
export class ConfigurationValueService implements ICrudService<ConfigurationValue> {
  private configurationValuesUrl = `${this.configurationService.apiUrl}/v1/configuration`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getConfigurationValues(
    filter: ConfigurationValueFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<ConfigurationValueListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.configurationValuesUrl, { headers: headers })
      .map(this.toConfigurationValues);
  }

  public get(id: string): Observable<ConfigurationValue> {
    const url = `${this.configurationValuesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toConfigurationValue);
  }

  public search(filter: ConfigurationValueFilter): Observable<PagedResult<ConfigurationValueListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.configurationValuesUrl, { headers: headers })
      .map(this.toConfigurationValues);
  }

  public getAllConfigurationValues(): Observable<ConfigurationValueListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('key', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.configurationValuesUrl, { headers: headers })
      .map(this.toConfigurationValues)
      .map(pagedResult => pagedResult.data);
  }

  public create(configurationValue: ConfigurationValue): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.configurationValuesUrl, JSON.stringify(configurationValue), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(configurationValue: ConfigurationValue): Observable<string> {
    const url = `${this.configurationValuesUrl}/${configurationValue.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(configurationValue), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toConfigurationValue(res: Response): ConfigurationValue {
    let body = res.json();
    return body || {} as ConfigurationValue;
  }

  private toConfigurationValues(res: Response): PagedResult<ConfigurationValueListItem> {
    return new PagedResultFactory<ConfigurationValueListItem>().create(res.headers, res.json());
  }
}
