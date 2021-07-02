import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  RegulationTypeFilter,
  RegulationTypeListItem,
  RegulationType
} from './';

@Injectable()
export class RegulationTypeService implements ICrudService<RegulationType> {
  private regulationTypesUrl = `${this.configurationService.apiUrl}/v1/regulationtypes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getRegulationTypes(
    filter: RegulationTypeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<RegulationTypeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.regulationTypesUrl, { headers: headers })
      .map(this.toRegulationTypes);
  }

  public get(id: string): Observable<RegulationType> {
    const url = `${this.regulationTypesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toRegulationType);
  }

  public search(filter: RegulationTypeFilter): Observable<PagedResult<RegulationTypeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.regulationTypesUrl, { headers: headers })
      .map(this.toRegulationTypes);
  }

  public getAllRegulationTypes(): Observable<RegulationTypeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.regulationTypesUrl, { headers: headers })
      .map(this.toRegulationTypes)
      .map(pagedResult => pagedResult.data);
  }

  public create(regulationType: RegulationType): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.regulationTypesUrl, JSON.stringify(regulationType), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(regulationType: RegulationType): Observable<string> {
    const url = `${this.regulationTypesUrl}/${regulationType.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(regulationType), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toRegulationType(res: Response): RegulationType {
    let body = res.json();
    return body || {} as RegulationType;
  }

  private toRegulationTypes(res: Response): PagedResult<RegulationTypeListItem> {
    return new PagedResultFactory<RegulationTypeListItem>().create(res.headers, res.json());
  }
}
