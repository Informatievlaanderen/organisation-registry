import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { BodyContactListItem } from './body-contact-list-item.model';
import { BodyContact } from './body-contact.model';
import { BodyContactFilter } from './body-contact-filter.model';

import { CreateBodyContactRequest, UpdateBodyContactRequest } from './';

@Injectable()
export class BodyContactService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getBodyContacts(
    bodyId: string,
    filter: BodyContactFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BodyContactListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getBodyContactsUrl(bodyId), { headers: headers })
      .map(this.toBodyContacts);
  }

  public get(bodyId: string, bodyContactId: string): Observable<BodyContact> {
    const url = `${this.getBodyContactsUrl(bodyId)}/${bodyContactId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyContact);
  }

  public create(bodyId, bodyContact: CreateBodyContactRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getBodyContactsUrl(bodyId), JSON.stringify(bodyContact), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(bodyId, bodyContact: UpdateBodyContactRequest): Observable<boolean> {
    const url = `${this.getBodyContactsUrl(bodyId)}/${bodyContact.bodyContactId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(bodyContact), { headers: headers })
      .map(response => response.ok);
  }

  private getBodyContactsUrl(bodyId) {
    return `${this.configurationService.apiUrl}/v1/bodies/${bodyId}/contacts`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toBodyContact(res: Response): BodyContact {
    let body = res.json();
    return body || {} as BodyContact;
  }

  private toBodyContacts(res: Response): PagedResult<BodyContactListItem> {
    return new PagedResultFactory<BodyContactListItem>().create(res.headers, res.json());
  }
}
