import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationKeyListItem } from './organisation-key-list-item.model';
import { OrganisationKey } from './organisation-key.model';
import { OrganisationKeyFilter } from './organisation-key-filter.model';

import { CreateOrganisationKeyRequest, UpdateOrganisationKeyRequest } from './';

@Injectable()
export class OrganisationKeyService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationKeys(
    organisationId: string,
    filter: OrganisationKeyFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationKeyListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationKeysUrl(organisationId), { headers: headers })
      .map(this.toOrganisationKeys);
  }

  public get(organisationId: string, organisationKeyId: string): Observable<OrganisationKey> {
    const url = `${this.getOrganisationKeysUrl(organisationId)}/${organisationKeyId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationKey);
  }

  public create(organisationId, organisationKey: CreateOrganisationKeyRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationKeysUrl(organisationId), JSON.stringify(organisationKey), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationKey: UpdateOrganisationKeyRequest): Observable<boolean> {
    const url = `${this.getOrganisationKeysUrl(organisationId)}/${organisationKey.organisationKeyId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationKey), { headers: headers })
      .map(response => response.ok);
  }

  private getOrganisationKeysUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/keys`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationKey(res: Response): OrganisationKey {
    let body = res.json();
    return body || {} as OrganisationKey;
  }

  private toOrganisationKeys(res: Response): PagedResult<OrganisationKeyListItem> {
    return new PagedResultFactory<OrganisationKeyListItem>().create(res.headers, res.json());
  }
}
