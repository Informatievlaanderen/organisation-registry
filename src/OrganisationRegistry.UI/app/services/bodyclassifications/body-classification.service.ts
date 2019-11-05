import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  BodyClassificationFilter,
  BodyClassificationListItem,
  BodyClassification
} from './';

@Injectable()
export class BodyClassificationService implements ICrudService<BodyClassification> {
  private bodyClassificationsUrl = `${this.configurationService.apiUrl}/v1/bodyclassifications`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getBodyClassifications(
    filter: BodyClassificationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BodyClassificationListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.bodyClassificationsUrl, { headers: headers })
      .map(this.toBodyClassifications);
  }

  public get(id: string): Observable<BodyClassification> {
    const url = `${this.bodyClassificationsUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyClassification);
  }

  public search(filter: BodyClassificationFilter): Observable<PagedResult<BodyClassificationListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.bodyClassificationsUrl, { headers: headers })
      .map(this.toBodyClassifications);
  }

  public getAllBodyClassifications(bodyClassificationTypeId: string): Observable<BodyClassificationListItem[]> {
    let search = new BodyClassificationFilter();
    search.bodyClassificationTypeId = bodyClassificationTypeId;


    let headers = new HeadersBuilder()
      .json()
      .withFiltering(search)
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.bodyClassificationsUrl, { headers: headers })
      .map(this.toBodyClassifications)
      .map(pagedResult => pagedResult.data);
  }

  public create(bodyClassification: BodyClassification): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.bodyClassificationsUrl, JSON.stringify(bodyClassification), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(bodyClassification: BodyClassification): Observable<string> {
    const url = `${this.bodyClassificationsUrl}/${bodyClassification.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(bodyClassification), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toBodyClassification(res: Response): BodyClassification {
    let body = res.json();
    return body || {} as BodyClassification;
  }

  private toBodyClassifications(res: Response): PagedResult<BodyClassificationListItem> {
    return new PagedResultFactory<BodyClassificationListItem>().create(res.headers, res.json());
  }
}
