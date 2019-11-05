import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationFunctionListItem } from './organisation-function-list-item.model';
import { OrganisationFunction } from './organisation-function.model';
import { OrganisationFunctionFilter } from './organisation-function-filter.model';

import { CreateOrganisationFunctionRequest, UpdateOrganisationFunctionRequest } from './';

@Injectable()
export class OrganisationFunctionService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationFunctions(
    organisationId: string,
    filter: OrganisationFunctionFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationFunctionListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationFunctionsUrl(organisationId), { headers: headers })
      .map(this.toOrganisationFunctions);
  }

  public get(organisationId: string, organisationFunctionId: string): Observable<OrganisationFunction> {
    const url = `${this.getOrganisationFunctionsUrl(organisationId)}/${organisationFunctionId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationFunction);
  }

  public create(organisationId, organisationFunction: CreateOrganisationFunctionRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationFunctionsUrl(organisationId), JSON.stringify(organisationFunction), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationFunction: UpdateOrganisationFunctionRequest): Observable<boolean> {
    const url = `${this.getOrganisationFunctionsUrl(organisationId)}/${organisationFunction.organisationFunctionId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationFunction), { headers: headers })
      .map(response => response.ok);
  }

  private getOrganisationFunctionsUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/functions`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationFunction(res: Response): OrganisationFunction {
    let body = res.json();
    return body || {} as OrganisationFunction;
  }

  private toOrganisationFunctions(res: Response): PagedResult<OrganisationFunctionListItem> {
    return new PagedResultFactory<OrganisationFunctionListItem>().create(res.headers, res.json());
  }
}
