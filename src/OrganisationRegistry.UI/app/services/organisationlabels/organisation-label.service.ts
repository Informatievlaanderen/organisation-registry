import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationLabelListItem } from './organisation-label-list-item.model';
import { OrganisationLabel } from './organisation-label.model';
import { OrganisationLabelFilter } from './organisation-label-filter.model';

import { CreateOrganisationLabelRequest, UpdateOrganisationLabelRequest } from './';

@Injectable()
export class OrganisationLabelService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationLabels(
    organisationId: string,
    filter: OrganisationLabelFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationLabelListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationLabelsUrl(organisationId), { headers: headers })
      .map(this.toOrganisationLabels);
  }

  public get(organisationId: string, organisationLabelId: string): Observable<OrganisationLabel> {
    const url = `${this.getOrganisationLabelsUrl(organisationId)}/${organisationLabelId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationLabel);
  }

  public create(organisationId, organisationLabel: CreateOrganisationLabelRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationLabelsUrl(organisationId), JSON.stringify(organisationLabel), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationLabel: UpdateOrganisationLabelRequest): Observable<boolean> {
    const url = `${this.getOrganisationLabelsUrl(organisationId)}/${organisationLabel.organisationLabelId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationLabel), { headers: headers })
      .map(response => response.ok);
  }

  private getOrganisationLabelsUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/labels`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationLabel(res: Response): OrganisationLabel {
    let body = res.json();
    return body || {} as OrganisationLabel;
  }

  private toOrganisationLabels(res: Response): PagedResult<OrganisationLabelListItem> {
    return new PagedResultFactory<OrganisationLabelListItem>().create(res.headers, res.json());
  }
}
