import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { BodyFormalFrameworkListItem } from './body-formal-framework-list-item.model';
import { BodyFormalFramework } from './body-formal-framework.model';
import { BodyFormalFrameworkFilter } from './body-formal-framework-filter.model';

import { CreateBodyFormalFrameworkRequest, UpdateBodyFormalFrameworkRequest } from './';

@Injectable()
export class BodyFormalFrameworkService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getBodyFormalFrameworks(
    bodyId: string,
    filter: BodyFormalFrameworkFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BodyFormalFrameworkListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getBodyFormalFrameworksUrl(bodyId), { headers: headers })
      .map(this.toBodyFormalFrameworks);
  }

  public get(bodyId: string, bodyFormalFrameworkId: string): Observable<BodyFormalFramework> {
    const url = `${this.getBodyFormalFrameworksUrl(bodyId)}/${bodyFormalFrameworkId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyFormalFramework);
  }

  public create(bodyId, bodyFormalFramework: CreateBodyFormalFrameworkRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getBodyFormalFrameworksUrl(bodyId), JSON.stringify(bodyFormalFramework), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(bodyId, bodyFormalFramework: UpdateBodyFormalFrameworkRequest): Observable<boolean> {
    const url = `${this.getBodyFormalFrameworksUrl(bodyId)}/${bodyFormalFramework.bodyFormalFrameworkId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(bodyFormalFramework), { headers: headers })
      .map(response => response.ok);
  }

  private getBodyFormalFrameworksUrl(bodyId) {
    return `${this.configurationService.apiUrl}/v1/bodies/${bodyId}/formalframeworks`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toBodyFormalFramework(res: Response): BodyFormalFramework {
    let body = res.json();
    return body || {} as BodyFormalFramework;
  }

  private toBodyFormalFrameworks(res: Response): PagedResult<BodyFormalFrameworkListItem> {
    return new PagedResultFactory<BodyFormalFrameworkListItem>().create(res.headers, res.json());
  }
}
