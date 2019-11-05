import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  LifecyclePhaseTypeFilter,
  LifecyclePhaseTypeListItem,
  LifecyclePhaseType
} from './';

@Injectable()
export class LifecyclePhaseTypeService implements ICrudService<LifecyclePhaseType> {
  private lifecyclePhaseTypesUrl = `${this.configurationService.apiUrl}/v1/lifecyclephasetypes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getLifecyclePhaseTypes(
    filter: LifecyclePhaseTypeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<LifecyclePhaseTypeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.lifecyclePhaseTypesUrl, { headers: headers })
      .map(this.toLifecyclePhaseTypes);
  }

  public get(id: string): Observable<LifecyclePhaseType> {
    const url = `${this.lifecyclePhaseTypesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toLifecyclePhaseType);
  }

  public search(filter: LifecyclePhaseTypeFilter): Observable<PagedResult<LifecyclePhaseTypeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.lifecyclePhaseTypesUrl, { headers: headers })
      .map(this.toLifecyclePhaseTypes);
  }

  public getAllLifecyclePhaseTypes(): Observable<LifecyclePhaseTypeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.lifecyclePhaseTypesUrl, { headers: headers })
      .map(this.toLifecyclePhaseTypes)
      .map(pagedResult => pagedResult.data);
  }

  public create(lifecyclePhaseType: LifecyclePhaseType): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.lifecyclePhaseTypesUrl, JSON.stringify(lifecyclePhaseType), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(lifecyclePhaseType: LifecyclePhaseType): Observable<string> {
    const url = `${this.lifecyclePhaseTypesUrl}/${lifecyclePhaseType.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(lifecyclePhaseType), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toLifecyclePhaseType(res: Response): LifecyclePhaseType {
    let body = res.json();
    return body || {} as LifecyclePhaseType;
  }

  private toLifecyclePhaseTypes(res: Response): PagedResult<LifecyclePhaseTypeListItem> {
    return new PagedResultFactory<LifecyclePhaseTypeListItem>().create(res.headers, res.json());
  }
}
