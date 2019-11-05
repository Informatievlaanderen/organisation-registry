import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationOrganisationClassificationListItem } from './organisation-organisationclassification-list-item.model';
import { OrganisationOrganisationClassification } from './organisation-organisationclassification.model';
import { OrganisationOrganisationClassificationFilter } from './organisation-organisationclassification-filter.model';

import { CreateOrganisationOrganisationClassificationRequest, UpdateOrganisationOrganisationClassificationRequest } from './';

@Injectable()
export class OrganisationOrganisationClassificationService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationOrganisationClassifications(
    organisationId: string,
    filter: OrganisationOrganisationClassificationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationOrganisationClassificationListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationOrganisationClassificationsUrl(organisationId), { headers: headers })
      .map(this.toOrganisationOrganisationClassifications);
  }

  public get(organisationId: string, organisationOrganisationClassificationId: string): Observable<OrganisationOrganisationClassification> {
    const url = `${this.getOrganisationOrganisationClassificationsUrl(organisationId)}/${organisationOrganisationClassificationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationOrganisationClassification);
  }

  public create(organisationId, organisationOrganisationClassification: CreateOrganisationOrganisationClassificationRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationOrganisationClassificationsUrl(organisationId), JSON.stringify(organisationOrganisationClassification), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationOrganisationClassification: UpdateOrganisationOrganisationClassificationRequest): Observable<boolean> {
    const url = `${this.getOrganisationOrganisationClassificationsUrl(organisationId)}/${organisationOrganisationClassification.organisationOrganisationClassificationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationOrganisationClassification), { headers: headers })
      .map(response => response.ok);
  }

  private getOrganisationOrganisationClassificationsUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/classifications`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationOrganisationClassification(res: Response): OrganisationOrganisationClassification {
    let body = res.json();
    return body || {} as OrganisationOrganisationClassification;
  }

  private toOrganisationOrganisationClassifications(res: Response): PagedResult<OrganisationOrganisationClassificationListItem> {
    return new PagedResultFactory<OrganisationOrganisationClassificationListItem>().create(res.headers, res.json());
  }
}
