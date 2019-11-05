import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationContactListItem } from './organisation-contact-list-item.model';
import { OrganisationContact } from './organisation-contact.model';
import { OrganisationContactFilter } from './organisation-contact-filter.model';

import { CreateOrganisationContactRequest, UpdateOrganisationContactRequest } from './';

@Injectable()
export class OrganisationContactService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationContacts(
    organisationId: string,
    filter: OrganisationContactFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationContactListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationContactsUrl(organisationId), { headers: headers })
      .map(this.toOrganisationContacts);
  }

  public get(organisationId: string, organisationContactId: string): Observable<OrganisationContact> {
    const url = `${this.getOrganisationContactsUrl(organisationId)}/${organisationContactId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationContact);
  }

  public create(organisationId, organisationContact: CreateOrganisationContactRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationContactsUrl(organisationId), JSON.stringify(organisationContact), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationContact: UpdateOrganisationContactRequest): Observable<boolean> {
    const url = `${this.getOrganisationContactsUrl(organisationId)}/${organisationContact.organisationContactId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationContact), { headers: headers })
      .map(response => response.ok);
  }

  private getOrganisationContactsUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/contacts`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationContact(res: Response): OrganisationContact {
    let body = res.json();
    return body || {} as OrganisationContact;
  }

  private toOrganisationContacts(res: Response): PagedResult<OrganisationContactListItem> {
    return new PagedResultFactory<OrganisationContactListItem>().create(res.headers, res.json());
  }
}
