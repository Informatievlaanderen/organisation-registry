import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { BodyOrganisationListItem } from './body-organisation-list-item.model';
import { BodyOrganisation } from './body-organisation.model';
import { BodyOrganisationFilter } from './body-organisation-filter.model';

import { CreateBodyOrganisationRequest, UpdateBodyOrganisationRequest } from './';

@Injectable()
export class BodyOrganisationService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getBodyOrganisations(
    bodyId: string,
    filter: BodyOrganisationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BodyOrganisationListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getBodyOrganisationsUrl(bodyId), { headers: headers })
      .map(this.toBodyOrganisations);
  }

  public get(bodyId: string, bodyOrganisationId: string): Observable<BodyOrganisation> {
    const url = `${this.getBodyOrganisationsUrl(bodyId)}/${bodyOrganisationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyOrganisation);
  }

  public create(bodyId, bodyOrganisation: CreateBodyOrganisationRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getBodyOrganisationsUrl(bodyId), JSON.stringify(bodyOrganisation), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(bodyId, bodyOrganisation: UpdateBodyOrganisationRequest): Observable<boolean> {
    const url = `${this.getBodyOrganisationsUrl(bodyId)}/${bodyOrganisation.bodyOrganisationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(bodyOrganisation), { headers: headers })
      .map(response => response.ok);
  }

  private getBodyOrganisationsUrl(bodyId) {
    return `${this.configurationService.apiUrl}/v1/bodies/${bodyId}/organisations`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toBodyOrganisation(res: Response): BodyOrganisation {
    let body = res.json();
    return body || {} as BodyOrganisation;
  }

  private toBodyOrganisations(res: Response): PagedResult<BodyOrganisationListItem> {
    return new PagedResultFactory<BodyOrganisationListItem>().create(res.headers, res.json());
  }
}
