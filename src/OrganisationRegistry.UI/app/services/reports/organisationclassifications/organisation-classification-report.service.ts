import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  OrganisationClassificationTranslationReportFilter,
  OrganisationClassificationTranslationReportListItem,
  OrganisationClassificationReportFilter,
  OrganisationClassificationReportListItem
} from './';

@Injectable()
export class OrganisationClassificationReportService {
  private classificationOrganisationsReportUrl = `${this.configurationService.apiUrl}/v1/reports`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getClassificationOrganisations(
    classificationId: string,
    filter: OrganisationClassificationTranslationReportFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationClassificationTranslationReportListItem>> {

    const url = `${this.classificationOrganisationsReportUrl}/classificationorganisations/${classificationId}`;

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
    classificationId: string,
    filter: OrganisationClassificationTranslationReportFilter,
    sortBy: string,
    sortOrder: SortOrder): Observable<string> {

    const url = `${this.classificationOrganisationsReportUrl}/classificationorganisations/${classificationId}`;

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

  public search(classificationId: string, filter: OrganisationClassificationTranslationReportFilter): Observable<PagedResult<OrganisationClassificationTranslationReportListItem>> {
    const url = `${this.classificationOrganisationsReportUrl}/classificationorganisations/${classificationId}`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toCapacityPersons);
  }

  public getPolicyDomainClassifications(
    filter: OrganisationClassificationReportFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationClassificationReportListItem>> {

    const url = `${this.classificationOrganisationsReportUrl}/policydomainclassifications`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationClassifications);
  }

  public getResponsibleMinisterClassifications(
    filter: OrganisationClassificationReportFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationClassificationReportListItem>> {

    const url = `${this.classificationOrganisationsReportUrl}/responsibleministerclassifications`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationClassifications);
  }  

  private toCapacityPersons(res: Response): PagedResult<OrganisationClassificationTranslationReportListItem> {
    return new PagedResultFactory<OrganisationClassificationTranslationReportListItem>().create(res.headers, res.json());
  }

  private toOrganisationClassifications(res: Response): PagedResult<OrganisationClassificationReportListItem> {
    return new PagedResultFactory<OrganisationClassificationReportListItem>().create(res.headers, res.json());
  }
}
