import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { PersonCapacityListItem } from './person-capacity-list-item.model';
import { PersonCapacity } from './person-capacity.model';

@Injectable()
export class PersonCapacityService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getPersonCapacities(
    personId: string,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<PersonCapacityListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getPersonCapacitiesUrl(personId), { headers: headers })
      .map(this.toPersonCapacities);
  }

  public getAllPersonCapacities(personId: string): Observable<PersonCapacityListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('capacityName', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.getPersonCapacitiesUrl(personId), { headers: headers })
      .map(this.toPersonCapacities)
      .map(pagedResult => pagedResult.data);
  }

  private getPersonCapacitiesUrl(personId) {
    return `${this.configurationService.apiUrl}/v1/people/${personId}/capacities`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toPersonCapacity(res: Response): PersonCapacity {
    let body = res.json();
    return body || {} as PersonCapacity;
  }

  private toPersonCapacities(res: Response): PagedResult<PersonCapacityListItem> {
    return new PagedResultFactory<PersonCapacityListItem>().create(res.headers, res.json());
  }
}
