import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  FormalFrameworkFilter,
  FormalFrameworkListItem,
  FormalFramework
} from './';

@Injectable()
export class FormalFrameworkService implements ICrudService<FormalFramework> {
  private formalFrameworksUrl = `${this.configurationService.apiUrl}/v1/formalframeworks`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getFormalFrameworks(
    filter: FormalFrameworkFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<FormalFrameworkListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.formalFrameworksUrl, { headers: headers })
      .map(this.toFormalFrameworks);
  }

  public getVademecumFormalFrameworks(
    filter: FormalFrameworkFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<FormalFrameworkListItem>> {

    const url = `${this.formalFrameworksUrl}/vademecum`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toFormalFrameworks);
  }

  public get(id: string): Observable<FormalFramework> {
    const url = `${this.formalFrameworksUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toFormalFramework);
  }

    public search(filter: FormalFrameworkFilter): Observable<PagedResult<FormalFrameworkListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.formalFrameworksUrl, { headers: headers })
      .map(this.toFormalFrameworks);
  }

  public getAllFormalFrameworks(): Observable<FormalFrameworkListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.formalFrameworksUrl, { headers: headers })
      .map(this.toFormalFrameworks)
      .map(pagedResult => pagedResult.data);
  }

  public create(formalFramework: FormalFramework): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.formalFrameworksUrl, JSON.stringify(formalFramework), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(formalFramework: FormalFramework): Observable<string> {
    const url = `${this.formalFrameworksUrl}/${formalFramework.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(formalFramework), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toFormalFramework(res: Response): FormalFramework {
    let body = res.json();
    return body || {} as FormalFramework;
  }

  private toFormalFrameworks(res: Response): PagedResult<FormalFrameworkListItem> {
    return new PagedResultFactory<FormalFrameworkListItem>().create(res.headers, res.json());
  }
}
