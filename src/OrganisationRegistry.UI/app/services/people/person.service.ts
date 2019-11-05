import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  PersonFilter,
  PersonListItem,
  Person
} from './';

@Injectable()
export class PersonService implements ICrudService<Person> {
  private peopleUrl = `${this.configurationService.apiUrl}/v1/people`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getPeople(
    filter: PersonFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<PersonListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.peopleUrl, { headers: headers })
      .map(this.toPeople);
  }

  public get(id: string): Observable<Person> {
    const url = `${this.peopleUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toPerson);
  }

  public search(filter: PersonFilter): Observable<PagedResult<PersonListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.peopleUrl, { headers: headers })
      .map(this.toPeople);
  }

  public getAllPeople(): Observable<PersonListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.peopleUrl, { headers: headers })
      .map(this.toPeople)
      .map(pagedResult => pagedResult.data);
  }

  public exportCsv(
    filter: PersonFilter,
    sortBy: string,
    sortOrder: SortOrder): Observable<string> {

    let headers = new HeadersBuilder()
      .csv()
      .withFiltering(filter)
      .withoutPagination()
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.peopleUrl, { headers: headers })
      .map(r => r.text());
  }

  public create(person: Person): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.peopleUrl, JSON.stringify(person), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(person: Person): Observable<string> {
    const url = `${this.peopleUrl}/${person.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(person), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toPerson(res: Response): Person {
    let body = res.json();
    return body || {} as Person;
  }

  private toPeople(res: Response): PagedResult<PersonListItem> {
    return new PagedResultFactory<PersonListItem>().create(res.headers, res.json());
  }
}
