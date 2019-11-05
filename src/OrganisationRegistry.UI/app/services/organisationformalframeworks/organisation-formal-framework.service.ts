import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationFormalFrameworkListItem } from './organisation-formal-framework-list-item.model';
import { OrganisationFormalFramework } from './organisation-formal-framework.model';
import { OrganisationFormalFrameworkFilter } from './organisation-formal-framework-filter.model';

import { CreateOrganisationFormalFrameworkRequest, UpdateOrganisationFormalFrameworkRequest } from './';

@Injectable()
export class OrganisationFormalFrameworkService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationFormalFrameworks(
    organisationId: string,
    filter: OrganisationFormalFrameworkFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationFormalFrameworkListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationFormalFrameworksUrl(organisationId), { headers: headers })
      .map(this.toOrganisationFormalFrameworks);
  }

  public get(organisationId: string, organisationFormalFrameworkId: string): Observable<OrganisationFormalFramework> {
    const url = `${this.getOrganisationFormalFrameworksUrl(organisationId)}/${organisationFormalFrameworkId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationFormalFramework);
  }

  public create(organisationId, organisationFormalFramework: CreateOrganisationFormalFrameworkRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationFormalFrameworksUrl(organisationId), JSON.stringify(organisationFormalFramework), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationFormalFramework: UpdateOrganisationFormalFrameworkRequest): Observable<boolean> {
    const url = `${this.getOrganisationFormalFrameworksUrl(organisationId)}/${organisationFormalFramework.organisationFormalFrameworkId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationFormalFramework), { headers: headers })
      .map(response => response.ok);
  }

  private getOrganisationFormalFrameworksUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/formalframeworks`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationFormalFramework(res: Response): OrganisationFormalFramework {
    let body = res.json();
    return body || {} as OrganisationFormalFramework;
  }

  private toOrganisationFormalFrameworks(res: Response): PagedResult<OrganisationFormalFrameworkListItem> {
    return new PagedResultFactory<OrganisationFormalFrameworkListItem>().create(res.headers, res.json());
  }
}
