import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  ContactTypeFilter,
  ContactTypeListItem,
  ContactType
} from './';

@Injectable()
export class ContactTypeService implements ICrudService<ContactType> {
  private contactTypesUrl = `${this.configurationService.apiUrl}/v1/contacttypes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getContactTypes(
    filter: ContactTypeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<ContactTypeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.contactTypesUrl, { headers: headers })
      .map(this.toContactTypes);
  }

  public get(id: string): Observable<ContactType> {
    const url = `${this.contactTypesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toContactType);
  }

  public search(filter: ContactTypeFilter): Observable<PagedResult<ContactTypeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.contactTypesUrl, { headers: headers })
      .map(this.toContactTypes);
  }

  public getAllContactTypes(): Observable<ContactTypeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.contactTypesUrl, { headers: headers })
      .map(this.toContactTypes)
      .map(pagedResult => pagedResult.data);
  }

  public create(contactType: ContactType): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.contactTypesUrl, JSON.stringify(contactType), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(contactType: ContactType): Observable<string> {
    const url = `${this.contactTypesUrl}/${contactType.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(contactType), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toContactType(res: Response): ContactType {
    let body = res.json();
    return body || {} as ContactType;
  }

  private toContactTypes(res: Response): PagedResult<ContactTypeListItem> {
    return new PagedResultFactory<ContactTypeListItem>().create(res.headers, res.json());
  }
}
