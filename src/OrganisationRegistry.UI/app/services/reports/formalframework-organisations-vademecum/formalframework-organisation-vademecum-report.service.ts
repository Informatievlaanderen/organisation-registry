import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  FormalFrameworkOrganisationVademecumReportFilter,
  FormalFrameworkOrganisationVademecumReportListItem
} from './';

@Injectable()
export class FormalFrameworkOrganisationVademecumReportService {
  private formalFrameworkOrganisationsReportUrl = `${this.configurationService.apiUrl}/v1/reports/formalframeworkorganisations/vademecum`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getFormalFrameworkOrganisations(
    formalFrameworkId: string,
    filter: FormalFrameworkOrganisationVademecumReportFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<FormalFrameworkOrganisationVademecumReportListItem>> {

    const url = `${this.formalFrameworkOrganisationsReportUrl}/${formalFrameworkId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toFormalFrameworkOrganisations);
  }

  public exportCsv(
    formalFrameworkId: string,
    filter: FormalFrameworkOrganisationVademecumReportFilter,
    sortBy: string,
    sortOrder: SortOrder): Observable<string> {

    const url = `${this.formalFrameworkOrganisationsReportUrl}/${formalFrameworkId}`;

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

  public search(formalFrameworkId: string, filter: FormalFrameworkOrganisationVademecumReportFilter): Observable<PagedResult<FormalFrameworkOrganisationVademecumReportListItem>> {
    const url = `${this.formalFrameworkOrganisationsReportUrl}/${formalFrameworkId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toFormalFrameworkOrganisations);
  }

  private toFormalFrameworkOrganisations(res: Response): PagedResult<FormalFrameworkOrganisationVademecumReportListItem> {
    return new PagedResultFactory<FormalFrameworkOrganisationVademecumReportListItem>().create(res.headers, res.json());
  }
}
