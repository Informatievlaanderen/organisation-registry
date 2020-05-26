import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  ParticipationSummaryReportFilter,
  ParticipationSummaryReportListItem
} from './';

@Injectable()
export class ParticipationSummaryReportService {
  private participationSummariesReportUrl = `${this.configurationService.apiUrl}/v1/reports/participationsummary`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getParticipationSummaries(
    filter: ParticipationSummaryReportFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<ParticipationSummaryReportListItem>> {

    const url = `${this.participationSummariesReportUrl}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toParticipationSummaries);
  }

  public exportCsv(
    filter: ParticipationSummaryReportFilter,
    sortBy: string,
    sortOrder: SortOrder): Observable<string> {

    const url = `${this.participationSummariesReportUrl}`;

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

  public search(filter: ParticipationSummaryReportFilter): Observable<PagedResult<ParticipationSummaryReportListItem>> {
    const url = `${this.participationSummariesReportUrl}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toParticipationSummaries);
  }

  private toParticipationSummaries(res: Response): PagedResult<ParticipationSummaryReportListItem> {
    return new PagedResultFactory<ParticipationSummaryReportListItem>().create(res.headers, res.json());
  }
}
