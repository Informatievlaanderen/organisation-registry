import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  BuildingFilter,
  BuildingListItem,
  Building
} from './';

@Injectable()
export class BuildingService implements ICrudService<Building> {
  private buildingsUrl = `${this.configurationService.apiUrl}/v1/buildings`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getBuildings(
    filter: BuildingFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BuildingListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.buildingsUrl, { headers: headers })
      .map(this.toBuildings);
  }

  public get(id: string): Observable<Building> {
    const url = `${this.buildingsUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBuilding);
  }

  public search(filter: BuildingFilter): Observable<PagedResult<BuildingListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.buildingsUrl, { headers: headers })
      .map(this.toBuildings);
  }

  public getAllBuildings(): Observable<BuildingListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      //.withFiltering(filter)
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.buildingsUrl, { headers: headers })
      .map(this.toBuildings)
      .map(pagedResult => pagedResult.data);
  }

  public create(building: Building): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.buildingsUrl, JSON.stringify(building), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(building: Building): Observable<string> {
    const url = `${this.buildingsUrl}/${building.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(building), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toBuilding(res: Response): Building {
    let body = res.json();
    return body || {} as Building;
  }

  private toBuildings(res: Response): PagedResult<BuildingListItem> {
    return new PagedResultFactory<BuildingListItem>().create(res.headers, res.json());
  }
}
