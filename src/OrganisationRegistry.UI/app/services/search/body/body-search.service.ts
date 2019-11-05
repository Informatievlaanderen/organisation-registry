import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  BodySearchListItem
} from './';

@Injectable()
export class BodySearchService {
  private url = `${this.configurationService.apiUrl}/v1/search/bodies`;

  private fields: string = 'id,bodyNumber,name,shortName';

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public search(
    query: string,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BodySearchListItem>> {

    return this.http
      .get(`${this.url}?q=${query}&offset=${page}&limit=${pageSize}&fields=${this.fields}`)
      .map(this.toBodies);
  }

  private toBodies(res: Response): PagedResult<BodySearchListItem> {
    return new PagedResultFactory<BodySearchListItem>().create(res.headers, res.json());
  }
}
