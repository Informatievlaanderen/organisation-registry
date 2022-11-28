import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationLocationListItem } from './organisation-location-list-item.model';
import { OrganisationLocation } from './organisation-location.model';
import { OrganisationLocationFilter } from './organisation-location-filter.model';

import { CreateOrganisationLocationRequest, UpdateOrganisationLocationRequest } from './';

@Injectable()
export class OrganisationLocationService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationLocations(
    organisationId: string,
    filter: OrganisationLocationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationLocationListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationLocationsUrl(organisationId), { headers: headers })
      .map(this.toOrganisationLocations);
  }

  public get(organisationId: string, organisationLocationId: string): Observable<OrganisationLocation> {
    const url = `${this.getOrganisationLocationsUrl(organisationId)}/${organisationLocationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationLocation);
  }

  public create(organisationId, organisationLocation: CreateOrganisationLocationRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationLocationsUrl(organisationId), JSON.stringify(organisationLocation), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationLocation: UpdateOrganisationLocationRequest): Observable<boolean> {
    const url = `${this.getOrganisationLocationsUrl(organisationId)}/${organisationLocation.locationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationLocation), { headers: headers })
      .map(response => response.ok);
  }

  delete(organisationId: string, organisationLocationId: string) {
    const url = `${this.getOrganisationLocationsUrl(
      organisationId
    )}/${organisationLocationId}`;
    let headers = new HeadersBuilder().json().build();

    return this.http.delete(url, { headers: headers });
  }

  private getOrganisationLocationsUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/locations`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationLocation(res: Response): OrganisationLocation {
    let body = res.json();
    return body || {} as OrganisationLocation;
  }

  private toOrganisationLocations(res: Response): PagedResult<OrganisationLocationListItem> {
    return new PagedResultFactory<OrganisationLocationListItem>().create(res.headers, res.json());
  }
}
