import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  FormalFrameworkParticipationReportFilter,
  FormalFrameworkParticipationReportListItem
} from './';

@Injectable()
export class FormalFrameworkParticipationReportService {
  private formalFrameworkParticipationsReportUrl = `${this.configurationService.apiUrl}/v1/reports/formalframeworkparticipation`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getParticipationsPerFormalFrameworkPerBody(
    formalFrameworkId: string,
    filter: FormalFrameworkParticipationReportFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<FormalFrameworkParticipationReportListItem>> {

    const url = `${this.formalFrameworkParticipationsReportUrl}/${formalFrameworkId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toFormalFrameworkParticipations);
  }

  public exportCsv(
    formalFrameworkId: string,
    filter: FormalFrameworkParticipationReportFilter,
    sortBy: string,
    sortOrder: SortOrder): Observable<string> {

    const url = `${this.formalFrameworkParticipationsReportUrl}/${formalFrameworkId}`;

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

  public search(formalFrameworkId: string, filter: FormalFrameworkParticipationReportFilter): Observable<PagedResult<FormalFrameworkParticipationReportListItem>> {
    const url = `${this.formalFrameworkParticipationsReportUrl}/${formalFrameworkId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toFormalFrameworkParticipations);
  }

  private toFormalFrameworkParticipations(res: Response): PagedResult<FormalFrameworkParticipationReportListItem> {
    return new PagedResultFactory<FormalFrameworkParticipationReportListItem>().create(res.headers, res.json());
  }
}
