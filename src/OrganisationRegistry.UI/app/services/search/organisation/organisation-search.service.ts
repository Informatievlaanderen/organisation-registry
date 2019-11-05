import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  OrganisationDocument
} from './';

@Injectable()
export class OrganisationSearchService {
  private url = `${this.configurationService.apiUrl}/v1/search/box/organisations`;

  private fields: string = 'id,name,ovoNumber,shortName,parents';

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public search(
    query: string,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationDocument>> {

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(`${this.url}?q=${query}&offset=${page}&limit=${pageSize}&fields=${this.fields}`, { headers: headers })
      .map(this.toOrganisations);
  }

  private toOrganisations(res: Response): PagedResult<OrganisationDocument> {
    return new PagedResultFactory<OrganisationDocument>().create(res.headers, res.json());
  }
}
