import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationRelationListItem } from './organisation-relation-list-item.model';
import { OrganisationRelation } from './organisation-relation.model';
import { OrganisationRelationFilter } from './organisation-relation-filter.model';

import { CreateOrganisationRelationRequest, UpdateOrganisationRelationRequest } from './';

@Injectable()
export class OrganisationRelationService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationRelations(
    organisationId: string,
    filter: OrganisationRelationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationRelationListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationRelationsUrl(organisationId), { headers: headers })
      .map(this.toOrganisationRelations);
  }

  public get(organisationId: string, organisationRelationId: string): Observable<OrganisationRelation> {
    const url = `${this.getOrganisationRelationsUrl(organisationId)}/${organisationRelationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationRelation);
  }

  public create(organisationId, organisationRelation: CreateOrganisationRelationRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationRelationsUrl(organisationId), JSON.stringify(organisationRelation), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationRelation: UpdateOrganisationRelationRequest): Observable<boolean> {
    const url = `${this.getOrganisationRelationsUrl(organisationId)}/${organisationRelation.organisationRelationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationRelation), { headers: headers })
      .map(response => response.ok);
  }

  private getOrganisationRelationsUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/relations`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationRelation(res: Response): OrganisationRelation {
    let body = res.json();
    return body || {} as OrganisationRelation;
  }

  private toOrganisationRelations(res: Response): PagedResult<OrganisationRelationListItem> {
    return new PagedResultFactory<OrganisationRelationListItem>().create(res.headers, res.json());
  }
}
