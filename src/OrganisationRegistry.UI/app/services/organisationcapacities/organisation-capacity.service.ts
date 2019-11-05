import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationCapacityListItem } from './organisation-capacity-list-item.model';
import { OrganisationCapacity } from './organisation-capacity.model';
import { OrganisationCapacityFilter } from './organisation-capacity-filter.model';

import { CreateOrganisationCapacityRequest, UpdateOrganisationCapacityRequest } from './';

@Injectable()
export class OrganisationCapacityService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationCapacities(
    organisationId: string,
    filter: OrganisationCapacityFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationCapacityListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationCapacitiesUrl(organisationId), { headers: headers })
      .map(this.toOrganisationCapacities);
  }

  public get(organisationId: string, organisationCapacityId: string): Observable<OrganisationCapacity> {
    const url = `${this.getOrganisationCapacitiesUrl(organisationId)}/${organisationCapacityId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationCapacity);
  }

  public create(organisationId, organisationCapacity: CreateOrganisationCapacityRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationCapacitiesUrl(organisationId), JSON.stringify(organisationCapacity), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationCapacity: UpdateOrganisationCapacityRequest): Observable<boolean> {
    const url = `${this.getOrganisationCapacitiesUrl(organisationId)}/${organisationCapacity.organisationCapacityId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationCapacity), { headers: headers })
      .map(response => response.ok);
  }

  private getOrganisationCapacitiesUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/capacities`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationCapacity(res: Response): OrganisationCapacity {
    let body = res.json();
    return body || {} as OrganisationCapacity;
  }

  private toOrganisationCapacities(res: Response): PagedResult<OrganisationCapacityListItem> {
    return new PagedResultFactory<OrganisationCapacityListItem>().create(res.headers, res.json());
  }
}
