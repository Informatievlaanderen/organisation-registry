import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  BodyClassificationTypeFilter,
  BodyClassificationTypeListItem,
  BodyClassificationType
} from './';

@Injectable()
export class BodyClassificationTypeService implements ICrudService<BodyClassificationType> {
  private bodyClassificationTypesUrl = `${this.configurationService.apiUrl}/v1/bodyclassificationtypes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getBodyClassificationTypes(
    filter: BodyClassificationTypeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BodyClassificationTypeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.bodyClassificationTypesUrl, { headers: headers })
      .map(this.toBodyClassificationTypes);
  }

  public get(id: string): Observable<BodyClassificationType> {
    const url = `${this.bodyClassificationTypesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyClassificationType);
  }

  public search(filter: BodyClassificationTypeFilter): Observable<PagedResult<BodyClassificationTypeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.bodyClassificationTypesUrl, { headers: headers })
      .map(this.toBodyClassificationTypes);
  }

  public getAllBodyClassificationTypes(): Observable<BodyClassificationTypeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.bodyClassificationTypesUrl, { headers: headers })
      .map(this.toBodyClassificationTypes)
      .map(pagedResult => pagedResult.data);
  }

  public create(bodyClassificationType: BodyClassificationType): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.bodyClassificationTypesUrl, JSON.stringify(bodyClassificationType), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(bodyClassificationType: BodyClassificationType): Observable<string> {
    const url = `${this.bodyClassificationTypesUrl}/${bodyClassificationType.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(bodyClassificationType), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toBodyClassificationType(res: Response): BodyClassificationType {
    let body = res.json();
    return body || {} as BodyClassificationType;
  }

  private toBodyClassificationTypes(res: Response): PagedResult<BodyClassificationTypeListItem> {
    return new PagedResultFactory<BodyClassificationTypeListItem>().create(res.headers, res.json());
  }
}
