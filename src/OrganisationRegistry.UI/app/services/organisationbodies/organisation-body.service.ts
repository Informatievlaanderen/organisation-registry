import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationBodyListItem } from './organisation-body-list-item.model';
import { OrganisationBody } from './organisation-body.model';
import { OrganisationBodyFilter } from './organisation-body-filter.model';

@Injectable()
export class OrganisationBodyService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationBodies(
    organisationId: string,
    filter: OrganisationBodyFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationBodyListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationBodiesUrl(organisationId), { headers: headers })
      .map(this.toOrganisationBodies);
  }

  private getOrganisationBodiesUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/bodies`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationBodies(res: Response): PagedResult<OrganisationBodyListItem> {
    return new PagedResultFactory<OrganisationBodyListItem>().create(res.headers, res.json());
  }
}
