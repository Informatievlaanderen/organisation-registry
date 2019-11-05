import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  OrganisationClassificationTypeFilter,
  OrganisationClassificationTypeListItem,
  OrganisationClassificationType
} from './';

@Injectable()
export class OrganisationClassificationTypeService implements ICrudService<OrganisationClassificationType> {
  private organisationClassificationTypesUrl = `${this.configurationService.apiUrl}/v1/organisationclassificationtypes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationClassificationTypes(
    filter: OrganisationClassificationTypeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationClassificationTypeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.organisationClassificationTypesUrl, { headers: headers })
      .map(this.toOrganisationClassificationTypes);
  }

  public get(id: string): Observable<OrganisationClassificationType> {
    const url = `${this.organisationClassificationTypesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationClassificationType);
  }

  public search(filter: OrganisationClassificationTypeFilter): Observable<PagedResult<OrganisationClassificationTypeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.organisationClassificationTypesUrl, { headers: headers })
      .map(this.toOrganisationClassificationTypes);
  }

  public getAllOrganisationClassificationTypes(): Observable<OrganisationClassificationTypeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.organisationClassificationTypesUrl, { headers: headers })
      .map(this.toOrganisationClassificationTypes)
      .map(pagedResult => pagedResult.data);
  }

  public getAllUserPermittedOrganisationClassificationTypes(): Observable<OrganisationClassificationTypeListItem[]> {
    return this.getAllOrganisationClassificationTypes().map(data => data.filter(classification => classification.userPermitted));
  }

  public create(organisationClassificationType: OrganisationClassificationType): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.organisationClassificationTypesUrl, JSON.stringify(organisationClassificationType), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationClassificationType: OrganisationClassificationType): Observable<string> {
    const url = `${this.organisationClassificationTypesUrl}/${organisationClassificationType.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationClassificationType), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationClassificationType(res: Response): OrganisationClassificationType {
    let body = res.json();
    return body || {} as OrganisationClassificationType;
  }

  private toOrganisationClassificationTypes(res: Response): PagedResult<OrganisationClassificationTypeListItem> {
    return new PagedResultFactory<OrganisationClassificationTypeListItem>().create(res.headers, res.json());
  }
}
