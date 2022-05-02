import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  KeyTypeFilter,
  KeyTypeListItem,
  KeyType
} from './';

@Injectable()
export class KeyTypeService implements ICrudService<KeyType> {
  private keyTypesUrl = `${this.configurationService.apiUrl}/v1/keytypes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getKeyTypes(
    filter: KeyTypeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<KeyTypeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.keyTypesUrl, { headers: headers })
      .map(this.toKeyTypes);
  }

  public get(id: string): Observable<KeyType> {
    const url = `${this.keyTypesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toKeyType);
  }

  public search(filter: KeyTypeFilter): Observable<PagedResult<KeyTypeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.keyTypesUrl, { headers: headers })
      .map(this.toKeyTypes);
  }

  public getAllKeyTypes(organisationId: string): Observable<KeyTypeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(`${this.keyTypesUrl}?forOrganisationId=${organisationId}`, { headers: headers })
      .map(this.toKeyTypes)
      .map(pagedResult => pagedResult.data.filter(x => x.userPermitted));
  }

  public create(keyType: KeyType): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.keyTypesUrl, JSON.stringify(keyType), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(keyType: KeyType): Observable<string> {
    const url = `${this.keyTypesUrl}/${keyType.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(keyType), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toKeyType(res: Response): KeyType {
    let body = res.json();
    return body || {} as KeyType;
  }

  private toKeyTypes(res: Response): PagedResult<KeyTypeListItem> {
    return new PagedResultFactory<KeyTypeListItem>().create(res.headers, res.json());
  }

  delete(keyType: KeyType) {
    const url = `${this.keyTypesUrl}/${keyType.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .delete(url, { headers: headers });
  }
}
