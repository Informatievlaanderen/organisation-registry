import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  CapacityPersonReportFilter,
  CapacityPersonReportListItem
} from './';

@Injectable()
export class CapacityPersonReportService {
  private capacityPersonsReportUrl = `${this.configurationService.apiUrl}/v1/reports/capacitypersons`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getCapacityPersons(
    capacityId: string,
    filter: CapacityPersonReportFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<CapacityPersonReportListItem>> {

    const url = `${this.capacityPersonsReportUrl}/${capacityId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toCapacityPersons);
  }

  public exportCsv(
    capacityId: string,
    filter: CapacityPersonReportFilter,
    sortBy: string,
    sortOrder: SortOrder): Observable<string> {

    const url = `${this.capacityPersonsReportUrl}/${capacityId}`;

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

  public search(capacityId: string, filter: CapacityPersonReportFilter): Observable<PagedResult<CapacityPersonReportListItem>> {
    const url = `${this.capacityPersonsReportUrl}/${capacityId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toCapacityPersons);
  }

  private toCapacityPersons(res: Response): PagedResult<CapacityPersonReportListItem> {
    return new PagedResultFactory<CapacityPersonReportListItem>().create(res.headers, res.json());
  }
}
