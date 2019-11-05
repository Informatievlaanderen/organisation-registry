import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { PersonMandateListItem } from './person-mandate-list-item.model';
import { PersonMandate } from './person-mandate.model';

@Injectable()
export class PersonMandateService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getPersonMandates(
    personId: string,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<PersonMandateListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getPersonMandatesUrl(personId), { headers: headers })
      .map(this.toPersonMandates);
  }

  public getAllPersonMandates(personId: string): Observable<PersonMandateListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('bodyName', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.getPersonMandatesUrl(personId), { headers: headers })
      .map(this.toPersonMandates)
      .map(pagedResult => pagedResult.data);
  }

  private getPersonMandatesUrl(personId) {
    return `${this.configurationService.apiUrl}/v1/people/${personId}/mandates`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toPersonMandate(res: Response): PersonMandate {
    let body = res.json();
    return body || {} as PersonMandate;
  }

  private toPersonMandates(res: Response): PagedResult<PersonMandateListItem> {
    return new PagedResultFactory<PersonMandateListItem>().create(res.headers, res.json());
  }
}
