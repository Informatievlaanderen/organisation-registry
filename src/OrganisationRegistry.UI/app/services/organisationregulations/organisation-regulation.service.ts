import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';

import { OrganisationRegulationListItem } from './organisation-regulation-list-item.model';
import { OrganisationRegulation } from './organisation-regulation.model';
import { OrganisationRegulationFilter } from './organisation-regulation-filter.model';

import { CreateOrganisationRegulationRequest, UpdateOrganisationRegulationRequest } from './';

@Injectable()
export class OrganisationRegulationService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationRegulations(
    organisationId: string,
    filter: OrganisationRegulationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationRegulationListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationRegulationsUrl(organisationId), { headers: headers })
      .map(this.toOrganisationRegulations);
  }

  public get(organisationId: string, organisationRegulationId: string): Observable<OrganisationRegulation> {
    const url = `${this.getOrganisationRegulationsUrl(organisationId)}/${organisationRegulationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationRegulation);
  }

  public create(organisationId, organisationRegulation: CreateOrganisationRegulationRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationRegulationsUrl(organisationId), JSON.stringify(organisationRegulation), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationRegulation: UpdateOrganisationRegulationRequest): Observable<boolean> {
    const url = `${this.getOrganisationRegulationsUrl(organisationId)}/${organisationRegulation.organisationRegulationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationRegulation), { headers: headers })
      .map(response => response.ok);
  }

  private getOrganisationRegulationsUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/regulations`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationRegulation(res: Response): OrganisationRegulation {
    let body = res.json();
    return body || {} as OrganisationRegulation;
  }

  private toOrganisationRegulations(res: Response): PagedResult<OrganisationRegulationListItem> {
    return new PagedResultFactory<OrganisationRegulationListItem>().create(res.headers, res.json());
  }
}
