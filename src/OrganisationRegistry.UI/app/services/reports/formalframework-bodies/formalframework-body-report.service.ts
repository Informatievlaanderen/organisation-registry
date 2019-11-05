import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  FormalFrameworkBodyReportFilter,
  FormalFrameworkBodyReportListItem
} from './';

@Injectable()
export class FormalFrameworkBodyReportService {
  private formalFrameworkBodiesReportUrl = `${this.configurationService.apiUrl}/v1/reports/formalframeworkbodies`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getFormalFrameworkBodies(
    formalFrameworkId: string,
    filter: FormalFrameworkBodyReportFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<FormalFrameworkBodyReportListItem>> {

    const url = `${this.formalFrameworkBodiesReportUrl}/${formalFrameworkId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toFormalFrameworkBodies);
  }

  public exportCsv(
    formalFrameworkId: string,
    filter: FormalFrameworkBodyReportFilter,
    sortBy: string,
    sortOrder: SortOrder): Observable<string> {

    const url = `${this.formalFrameworkBodiesReportUrl}/${formalFrameworkId}`;

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

  public search(formalFrameworkId: string, filter: FormalFrameworkBodyReportFilter): Observable<PagedResult<FormalFrameworkBodyReportListItem>> {
    const url = `${this.formalFrameworkBodiesReportUrl}/${formalFrameworkId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toFormalFrameworkBodies);
  }

  private toFormalFrameworkBodies(res: Response): PagedResult<FormalFrameworkBodyReportListItem> {
    return new PagedResultFactory<FormalFrameworkBodyReportListItem>().create(res.headers, res.json());
  }
}
