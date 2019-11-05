import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  OrganisationRelationType,
  OrganisationRelationTypeListItem,
  OrganisationRelationTypeFilter
} from './';

@Injectable()
export class OrganisationRelationTypeService implements ICrudService<OrganisationRelationType> {
  private organisationRelationTypesUrl = `${this.configurationService.apiUrl}/v1/organisationrelationtypes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationRelationTypes(
    filter: OrganisationRelationTypeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationRelationTypeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.organisationRelationTypesUrl, { headers: headers })
      .map(this.toOrganisationRelationTypes);
  }

  public get(id: string): Observable<OrganisationRelationType> {
    const url = `${this.organisationRelationTypesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationRelationType);
  }

  public search(filter: OrganisationRelationTypeFilter): Observable<PagedResult<OrganisationRelationTypeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.organisationRelationTypesUrl, { headers: headers })
      .map(this.toOrganisationRelationTypes);
  }

  public getAllOrganisationRelationTypes(): Observable<OrganisationRelationTypeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.organisationRelationTypesUrl, { headers: headers })
      .map(this.toOrganisationRelationTypes)
      .map(pagedResult => pagedResult.data);
  }

  public create(organisationRelationType: OrganisationRelationType): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.organisationRelationTypesUrl, JSON.stringify(organisationRelationType), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationRelationType: OrganisationRelationType): Observable<string> {
    const url = `${this.organisationRelationTypesUrl}/${organisationRelationType.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationRelationType), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationRelationType(res: Response): OrganisationRelationType {
    let body = res.json();
    return body || {} as OrganisationRelationType;
  }

  private toOrganisationRelationTypes(res: Response): PagedResult<OrganisationRelationTypeListItem> {
    return new PagedResultFactory<OrganisationRelationTypeListItem>().create(res.headers, res.json());
  }
}
