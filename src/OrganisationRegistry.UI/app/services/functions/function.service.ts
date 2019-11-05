import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  FunctionFilter,
  FunctionListItem,
  Function
} from './';

@Injectable()
export class FunctionService implements ICrudService<Function> {
  private functionsUrl = `${this.configurationService.apiUrl}/v1/functiontypes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getFunctions(
    filter: FunctionFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<FunctionListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.functionsUrl, { headers: headers })
      .map(this.toFunctions);
  }

  public get(id: string): Observable<Function> {
    const url = `${this.functionsUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toFunction);
  }

  public search(filter: FunctionFilter): Observable<PagedResult<FunctionListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.functionsUrl, { headers: headers })
      .map(this.toFunctions);
  }

  public getAllFunctions(): Observable<FunctionListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.functionsUrl, { headers: headers })
      .map(this.toFunctions)
      .map(pagedResult => pagedResult.data);
  }

  public create(aFunction: Function): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.functionsUrl, JSON.stringify(aFunction), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(aFunction: Function): Observable<string> {
    const url = `${this.functionsUrl}/${aFunction.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(aFunction), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toFunction(res: Response): Function {
    let body = res.json();
    return body || {} as Function;
  }

  private toFunctions(res: Response): PagedResult<FunctionListItem> {
    return new PagedResultFactory<FunctionListItem>().create(res.headers, res.json());
  }
}
