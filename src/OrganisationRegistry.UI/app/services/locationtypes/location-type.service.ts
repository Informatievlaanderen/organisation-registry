import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  LocationTypeFilter,
  LocationTypeListItem,
  LocationType
} from './';

@Injectable()
export class LocationTypeService implements ICrudService<LocationType> {
  private locationTypesUrl = `${this.configurationService.apiUrl}/v1/locationtypes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getLocationTypes(
    filter: LocationTypeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<LocationTypeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.locationTypesUrl, { headers: headers })
      .map(this.toLocationTypes);
  }

  public get(id: string): Observable<LocationType> {
    const url = `${this.locationTypesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toLocationType);
  }

  public search(filter: LocationTypeFilter): Observable<PagedResult<LocationTypeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.locationTypesUrl, { headers: headers })
      .map(this.toLocationTypes);
  }

  public getAllLocationTypes(): Observable<LocationTypeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.locationTypesUrl, { headers: headers })
      .map(this.toLocationTypes)
      .map(pagedResult => pagedResult.data);
  }

  public getAllUserPermittedLocationTypes(): Observable<LocationTypeListItem[]> {
    let headers = new HeadersBuilder()
    .json()
    .withoutPagination()
    .withSorting('name', SortOrder.Ascending)
    .build();

  return this.http
    .get(this.locationTypesUrl, { headers: headers })
    .map(this.toLocationTypes)
    .map(pagedResult => pagedResult.data.filter(x => x.userPermitted));
  }

  public create(locationType: LocationType): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.locationTypesUrl, JSON.stringify(locationType), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(locationType: LocationType): Observable<string> {
    const url = `${this.locationTypesUrl}/${locationType.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(locationType), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toLocationType(res: Response): LocationType {
    let body = res.json();
    return body || {} as LocationType;
  }

  private toLocationTypes(res: Response): PagedResult<LocationTypeListItem> {
    return new PagedResultFactory<LocationTypeListItem>().create(res.headers, res.json());
  }
}
