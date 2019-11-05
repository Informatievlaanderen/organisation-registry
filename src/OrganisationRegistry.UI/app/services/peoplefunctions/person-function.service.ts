import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { PersonFunctionListItem } from './person-function-list-item.model';
import { PersonFunction } from './person-function.model';

@Injectable()
export class PersonFunctionService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getPersonFunctions(
    personId: string,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<PersonFunctionListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getPersonFunctionsUrl(personId), { headers: headers })
      .map(this.toPersonFunctions);
  }

  public getAllPersonFunctions(personId: string): Observable<PersonFunctionListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('functionName', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.getPersonFunctionsUrl(personId), { headers: headers })
      .map(this.toPersonFunctions)
      .map(pagedResult => pagedResult.data);
  }

  private getPersonFunctionsUrl(personId) {
    return `${this.configurationService.apiUrl}/v1/people/${personId}/functions`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toPersonFunction(res: Response): PersonFunction {
    let body = res.json();
    return body || {} as PersonFunction;
  }

  private toPersonFunctions(res: Response): PagedResult<PersonFunctionListItem> {
    return new PagedResultFactory<PersonFunctionListItem>().create(res.headers, res.json());
  }
}
