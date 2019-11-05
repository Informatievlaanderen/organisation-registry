import { Injectable } from '@angular/core';
import { Response, Headers, Http, Jsonp } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  LocationFilter,
  LocationListItem,
  Location
} from './';

@Injectable()
export class LocationService implements ICrudService<Location> {
  private locationsUrl = `${this.configurationService.apiUrl}/v1/locations`;

  constructor(
    private http: Http,
    private jsonP: Jsonp,
    private configurationService: ConfigurationService
  ) { }

  public getLocations(
    filter: LocationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<LocationListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.locationsUrl, { headers: headers })
      .map(this.toLocations);
  }

  public search(filter: LocationFilter): Observable<PagedResult<LocationListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.locationsUrl, { headers: headers })
      .map(this.toLocations);
  }

  public getAllLocations(): Observable<LocationListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.locationsUrl, { headers: headers })
      .map(this.toLocations)
      .map(pagedResult => pagedResult.data);
  }

  public getCrabLocations(searchTerm: string): Observable<any[]> {
    return this.jsonP
      .request(`https://loc.geopunt.be/v3/Location?q=${searchTerm}&c=50&callback=JSONP_CALLBACK`, { method: 'Get' })
      .map(r => r.json().LocationResult);
  }

  public get(id: string): Observable<Location> {
    const url = `${this.locationsUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toLocation);
  }

  public create(location: Location): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.locationsUrl, JSON.stringify(location), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(location: Location): Observable<string> {
    const url = `${this.locationsUrl}/${location.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(location), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toLocation(res: Response): Location {
    let body = res.json();
    return body || {} as Location;
  }

  private toLocations(res: Response): PagedResult<LocationListItem> {
    return new PagedResultFactory<LocationListItem>().create(res.headers, res.json());
  }

}
