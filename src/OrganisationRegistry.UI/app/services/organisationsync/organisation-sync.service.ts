import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';

import { OrganisationTermination } from './organisation-termination.model';
import {PagedResult, PagedResultFactory, SortOrder} from "../../core/pagination";
import {OrganisationTerminationFilter} from "./organisation-termination-filter.model";
import {EventListItem} from "../events";

@Injectable()
export class OrganisationSyncService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public get(organisationId: string, kboNumber: string): Observable<OrganisationTermination> {
    const url = `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/kbo/${kboNumber}/termination`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationTermination);
  }

  public list(
    filter: OrganisationTerminationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationTermination>> {

    const url = `${this.configurationService.apiUrl}/v1/organisations/kbo/terminated`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationTerminations);
  }

  public sync(organisationId: string): Observable<boolean> {
    const url = `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/kbo/sync`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, '', { headers: headers })
      .map(response => response.ok);
  }

  public cancelCouplingWithKbo(organisationId: string) {
    const url = `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/kbo/cancel`;

    let headers = new HeadersBuilder().build();

    return this.http
      .put(url, '', { headers: headers });
  }

  public syncTermination(organisationId: string): Observable<boolean> {
    const url = `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/kbo/terminate`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, '', { headers: headers })
      .map(response => response.ok);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationTermination(res: Response): OrganisationTermination {
    let body = res.json();
    return body || {} as OrganisationTermination;
  }

  private toOrganisationTerminations(res: Response): PagedResult<OrganisationTermination> {
    return new PagedResultFactory<OrganisationTermination>().create(res.headers, res.json());
  }
}
