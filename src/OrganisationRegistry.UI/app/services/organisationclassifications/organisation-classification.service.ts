import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  OrganisationClassificationFilter,
  OrganisationClassificationListItem,
  OrganisationClassification
} from './';

@Injectable()
export class OrganisationClassificationService implements ICrudService<OrganisationClassification> {
  private organisationClassificationsUrl = `${this.configurationService.apiUrl}/v1/organisationclassifications`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationClassifications(
    filter: OrganisationClassificationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationClassificationListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.organisationClassificationsUrl, { headers: headers })
      .map(this.toOrganisationClassifications);
  }

/*
  public getPolicyDomainClassifications(
    filter: OrganisationClassificationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationClassificationListItem>> {

    const url = `${this.organisationClassificationsUrl}/policydomainclassifications`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationClassifications);
  }  
  */

  public get(id: string): Observable<OrganisationClassification> {
    const url = `${this.organisationClassificationsUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationClassification);
  }

  public search(filter: OrganisationClassificationFilter): Observable<PagedResult<OrganisationClassificationListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.organisationClassificationsUrl, { headers: headers })
      .map(this.toOrganisationClassifications);
  }

  public getAllOrganisationClassifications(organisationClassificationTypeId: string): Observable<OrganisationClassificationListItem[]> {
    let search = new OrganisationClassificationFilter();
    search.organisationClassificationTypeId = organisationClassificationTypeId;


    let headers = new HeadersBuilder()
      .json()
      .withFiltering(search)
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.organisationClassificationsUrl, { headers: headers })
      .map(this.toOrganisationClassifications)
      .map(pagedResult => pagedResult.data);
  }

  public create(organisationClassification: OrganisationClassification): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.organisationClassificationsUrl, JSON.stringify(organisationClassification), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationClassification: OrganisationClassification): Observable<string> {
    const url = `${this.organisationClassificationsUrl}/${organisationClassification.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationClassification), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationClassification(res: Response): OrganisationClassification {
    let body = res.json();
    return body || {} as OrganisationClassification;
  }

  private toOrganisationClassifications(res: Response): PagedResult<OrganisationClassificationListItem> {
    return new PagedResultFactory<OrganisationClassificationListItem>().create(res.headers, res.json());
  }
}
