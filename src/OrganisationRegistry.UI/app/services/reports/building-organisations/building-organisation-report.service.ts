import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  BuildingOrganisationReportFilter,
  BuildingOrganisationReportListItem
} from './';

@Injectable()
export class BuildingOrganisationReportService {
  private buildingOrganisationsReportUrl = `${this.configurationService.apiUrl}/v1/reports/buildingorganisations`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getBuildingOrganisations(
    buildingId: string,
    filter: BuildingOrganisationReportFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BuildingOrganisationReportListItem>> {

    const url = `${this.buildingOrganisationsReportUrl}/${buildingId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBuildingOrganisations);
  }

  public exportCsv(
    buildingId: string,
    filter: BuildingOrganisationReportFilter,
    sortBy: string,
    sortOrder: SortOrder): Observable<string> {

    const url = `${this.buildingOrganisationsReportUrl}/${buildingId}`;

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

  public search(buildingId: string, filter: BuildingOrganisationReportFilter): Observable<PagedResult<BuildingOrganisationReportListItem>> {
    const url = `${this.buildingOrganisationsReportUrl}/${buildingId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBuildingOrganisations);
  }

  private toBuildingOrganisations(res: Response): PagedResult<BuildingOrganisationReportListItem> {
    return new PagedResultFactory<BuildingOrganisationReportListItem>().create(res.headers, res.json());
  }
}
