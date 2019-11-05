import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationParentListItem } from './organisation-parent-list-item.model';
import { OrganisationParent } from './organisation-parent.model';

import { CreateOrganisationParentRequest, UpdateOrganisationParentRequest } from './';

@Injectable()
export class OrganisationParentService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationParents(
    organisationId: string,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationParentListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationParentsUrl(organisationId), { headers: headers })
      .map(this.toOrganisationParents);
  }

  public get(organisationId: string, organisationOrganisationParentId: string): Observable<OrganisationParent> {
    const url = `${this.getOrganisationParentsUrl(organisationId)}/${organisationOrganisationParentId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationParent);
  }

  public create(organisationId, organisationParent: CreateOrganisationParentRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationParentsUrl(organisationId), JSON.stringify(organisationParent), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationParent: UpdateOrganisationParentRequest): Observable<boolean> {
    const url = `${this.getOrganisationParentsUrl(organisationId)}/${organisationParent.organisationOrganisationParentId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationParent), { headers: headers })
      .map(response => response.ok);
  }

  private getOrganisationParentsUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/parents`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationParent(res: Response): OrganisationParent {
    let body = res.json();
    return body || {} as OrganisationParent;
  }

  private toOrganisationParents(res: Response): PagedResult<OrganisationParentListItem> {
    return new PagedResultFactory<OrganisationParentListItem>().create(res.headers, res.json());
  }
}
