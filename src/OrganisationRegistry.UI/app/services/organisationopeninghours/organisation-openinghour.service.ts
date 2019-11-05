import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationOpeningHourListItem } from './organisation-openinghour-list-item.model';
import { OrganisationOpeningHour } from './organisation-openinghour.model';
import { OrganisationOpeningHourFilter } from './organisation-openinghour-filter.model';

import { CreateOrganisationOpeningHourRequest, UpdateOrganisationOpeningHourRequest } from './';

@Injectable()
export class OrganisationOpeningHourService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationOpeningHours(
    organisationId: string,
    filter: OrganisationOpeningHourFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationOpeningHourListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationOpeningHoursUrl(organisationId), { headers: headers })
      .map(this.toOrganisationOpeningHours);
  }

  public get(organisationId: string, organisationOpeningHourId: string): Observable<OrganisationOpeningHour> {
    const url = `${this.getOrganisationOpeningHoursUrl(organisationId)}/${organisationOpeningHourId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationOpeningHour);
  }

  // public getAllDays(organisationId: string) {
  //   const url = `${this.getOrganisationOpeningHoursUrl(organisationId)}/days`;

  //   let headers = new HeadersBuilder()
  //     .json()
  //     .build();

  //   return this.http
  //     .get(url, { headers: headers })
  //     .map(res => res.json());
  // }

  // public getAllHours(organisationId: string) {
  //   const url = `${this.getOrganisationOpeningHoursUrl(organisationId)}/hours`;

  //   let headers = new HeadersBuilder()
  //     .json()
  //     .build();

  //   return this.http
  //     .get(url, { headers: headers })
  //     .map(res => res.json());
  // }

  public create(organisationId, organisationOpeningHour: CreateOrganisationOpeningHourRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationOpeningHoursUrl(organisationId), JSON.stringify(organisationOpeningHour), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationOpeningHour: UpdateOrganisationOpeningHourRequest): Observable<boolean> {
    const url = `${this.getOrganisationOpeningHoursUrl(organisationId)}/${organisationOpeningHour.organisationOpeningHourId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationOpeningHour), { headers: headers })
      .map(response => response.ok);
  }

  private getOrganisationOpeningHoursUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/openinghours`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationOpeningHour(res: Response): OrganisationOpeningHour {
    let body = res.json();
    return body || {} as OrganisationOpeningHour;
  }

  private toOrganisationOpeningHours(res: Response): PagedResult<OrganisationOpeningHourListItem> {
    return new PagedResultFactory<OrganisationOpeningHourListItem>().create(res.headers, res.json());
  }
}
