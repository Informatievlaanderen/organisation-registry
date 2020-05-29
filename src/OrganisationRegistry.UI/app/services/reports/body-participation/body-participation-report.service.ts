import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';

import {
  BodyParticipationReportFilter,
  BodyParticipationReportListItem,
  BodyParticipationReportTotals
} from './';

@Injectable()
export class BodyParticipationReportService {
  private bodyParticipationsReportUrl = `${this.configurationService.apiUrl}/v1/reports/bodyparticipation`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getParticipationsPerBody(
    bodyId: string,
    filter: BodyParticipationReportFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BodyParticipationReportListItem>> {

    const url = `${this.bodyParticipationsReportUrl}/${bodyId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyParticipations);
  }

  public getParticipationsPerBodyTotals(
    bodyId: string,
    filter: BodyParticipationReportFilter): Observable<BodyParticipationReportTotals> {

    const url = `${this.bodyParticipationsReportUrl}/${bodyId}/totals`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      // .withPagination(page, pageSize)
      // .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyParticipationTotals);
  }

  public exportCsv(
    bodyId: string,
    filter: BodyParticipationReportFilter,
    sortBy: string,
    sortOrder: SortOrder): Observable<string> {

    const url = `${this.bodyParticipationsReportUrl}/${bodyId}`;

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

  public search(bodyId: string, filter: BodyParticipationReportFilter): Observable<PagedResult<BodyParticipationReportListItem>> {
    const url = `${this.bodyParticipationsReportUrl}/${bodyId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyParticipations);
  }

  private toBodyParticipations(res: Response): PagedResult<BodyParticipationReportListItem> {
    return new PagedResultFactory<BodyParticipationReportListItem>().create(res.headers, res.json());
  }

  private toBodyParticipationTotals(res: Response): BodyParticipationReportTotals {
    let body = res.json();
    return (body || {}) as BodyParticipationReportTotals;
  }
}
