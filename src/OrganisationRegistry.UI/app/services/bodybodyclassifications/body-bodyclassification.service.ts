import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';

import { BodyBodyClassificationListItem } from './body-bodyclassification-list-item.model';
import { BodyBodyClassification } from './body-bodyclassification.model';
import { BodyBodyClassificationFilter } from './body-bodyclassification-filter.model';

import { CreateBodyBodyClassificationRequest, UpdateBodyBodyClassificationRequest } from './';

@Injectable()
export class BodyBodyClassificationService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getBodyBodyClassifications(
    bodyId: string,
    filter: BodyBodyClassificationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BodyBodyClassificationListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getBodyBodyClassificationsUrl(bodyId), { headers: headers })
      .map(this.toBodyBodyClassifications);
  }

  public get(bodyId: string, bodyBodyClassificationId: string): Observable<BodyBodyClassification> {
    const url = `${this.getBodyBodyClassificationsUrl(bodyId)}/${bodyBodyClassificationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyBodyClassification);
  }

  public create(bodyId, bodyBodyClassification: CreateBodyBodyClassificationRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getBodyBodyClassificationsUrl(bodyId), JSON.stringify(bodyBodyClassification), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(bodyId, bodyBodyClassification: UpdateBodyBodyClassificationRequest): Observable<boolean> {
    const url = `${this.getBodyBodyClassificationsUrl(bodyId)}/${bodyBodyClassification.bodyBodyClassificationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(bodyBodyClassification), { headers: headers })
      .map(response => response.ok);
  }

  private getBodyBodyClassificationsUrl(bodyId) {
    return `${this.configurationService.apiUrl}/v1/bodies/${bodyId}/classifications`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toBodyBodyClassification(res: Response): BodyBodyClassification {
    let body = res.json();
    return body || {} as BodyBodyClassification;
  }

  private toBodyBodyClassifications(res: Response): PagedResult<BodyBodyClassificationListItem> {
    return new PagedResultFactory<BodyBodyClassificationListItem>().create(res.headers, res.json());
  }
}
