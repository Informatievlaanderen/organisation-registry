import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  MandateRoleTypeFilter,
  MandateRoleTypeListItem,
  MandateRoleType
} from './';

@Injectable()
export class MandateRoleTypeService implements ICrudService<MandateRoleType> {
  private mandateRoleTypesUrl = `${this.configurationService.apiUrl}/v1/mandateroletypes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getMandateRoleTypes(
    filter: MandateRoleTypeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<MandateRoleTypeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.mandateRoleTypesUrl, { headers: headers })
      .map(this.toMandateRoleTypes);
  }

  public get(id: string): Observable<MandateRoleType> {
    const url = `${this.mandateRoleTypesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toMandateRoleType);
  }

  public search(filter: MandateRoleTypeFilter): Observable<PagedResult<MandateRoleTypeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.mandateRoleTypesUrl, { headers: headers })
      .map(this.toMandateRoleTypes);
  }

  public getAllMandateRoleTypes(): Observable<MandateRoleTypeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.mandateRoleTypesUrl, { headers: headers })
      .map(this.toMandateRoleTypes)
      .map(pagedResult => pagedResult.data);
  }

  public create(mandateRoleType: MandateRoleType): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.mandateRoleTypesUrl, JSON.stringify(mandateRoleType), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(mandateRoleType: MandateRoleType): Observable<string> {
    const url = `${this.mandateRoleTypesUrl}/${mandateRoleType.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(mandateRoleType), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toMandateRoleType(res: Response): MandateRoleType {
    let body = res.json();
    return body || {} as MandateRoleType;
  }

  private toMandateRoleTypes(res: Response): PagedResult<MandateRoleTypeListItem> {
    return new PagedResultFactory<MandateRoleTypeListItem>().create(res.headers, res.json());
  }
}
