import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  PersonSearchListItem
} from './';

@Injectable()
export class PersonSearchService {
  private url = `${this.configurationService.apiUrl}/v1/search/people`;

  private fields: string = 'id,name,firstName';

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public search(
    query: string,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<PersonSearchListItem>> {

    return this.http
      .get(`${this.url}?q=${query}&offset=${page}&limit=${pageSize}&fields=${this.fields}`)
      .map(this.toPersons);
  }

  private toPersons(res: Response): PagedResult<PersonSearchListItem> {
    return new PagedResultFactory<PersonSearchListItem>().create(res.headers, res.json());
  }
}
