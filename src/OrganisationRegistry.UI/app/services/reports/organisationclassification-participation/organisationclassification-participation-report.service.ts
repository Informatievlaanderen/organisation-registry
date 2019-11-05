import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  OrganisationClassificationParticipationReportFilter,
  OrganisationClassificationParticipationReportListItem
} from './';

@Injectable()
export class OrganisationClassificationParticipationReportService {
  private classificationOrganisationParticipationsReportUrl = `${this.configurationService.apiUrl}/v1/reports/classificationorganisationsparticipation`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getParticipationsPerOrganisationClassificationPerBody(
    classificationOrganisationId: string,
    filter: OrganisationClassificationParticipationReportFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationClassificationParticipationReportListItem>> {

    const url = `${this.classificationOrganisationParticipationsReportUrl}/${classificationOrganisationId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationClassificationParticipations);
  }

  public exportCsv(
    classificationOrganisationId: string,
    filter: OrganisationClassificationParticipationReportFilter,
    sortBy: string,
    sortOrder: SortOrder): Observable<string> {

    const url = `${this.classificationOrganisationParticipationsReportUrl}/${classificationOrganisationId}`;

    let headers = new HeadersBuilder()
      .csv()
      .withFiltering(filter)
      .withoutPagination()
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(r => r.text());
  }

  public search(
    classificationOrganisationId: string, filter: OrganisationClassificationParticipationReportFilter): 
      Observable<PagedResult<OrganisationClassificationParticipationReportListItem>> {

      const url = `${this.classificationOrganisationParticipationsReportUrl}/${classificationOrganisationId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationClassificationParticipations);
  }

  private toOrganisationClassificationParticipations(res: Response): PagedResult<OrganisationClassificationParticipationReportListItem> {
    return new PagedResultFactory<OrganisationClassificationParticipationReportListItem>().create(res.headers, res.json());
  }
}
