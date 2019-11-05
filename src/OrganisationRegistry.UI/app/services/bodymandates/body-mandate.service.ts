import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { BodyMandateListItem } from './body-mandate-list-item.model';
import { BodyMandate } from './body-mandate.model';
import { BodyMandateFilter } from './body-mandate-filter.model';

import { CreateBodyMandateRequest, UpdateBodyMandateRequest } from './';

@Injectable()
export class BodyMandateService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getBodyMandates(
    bodyId: string,
    filter: BodyMandateFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BodyMandateListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getBodyMandatesUrl(bodyId), { headers: headers })
      .map(this.toBodyMandates);
  }

  public get(bodyId: string, bodyMandateId: string): Observable<BodyMandate> {
    const url = `${this.getBodyMandatesUrl(bodyId)}/${bodyMandateId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyMandate);
  }

  public create(bodyId, bodyMandate: CreateBodyMandateRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getBodyMandatesUrl(bodyId), JSON.stringify(bodyMandate), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(bodyId, bodyMandate: UpdateBodyMandateRequest): Observable<boolean> {
    const url = `${this.getBodyMandatesUrl(bodyId)}/${bodyMandate.bodyMandateId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(bodyMandate), { headers: headers })
      .map(response => response.ok);
  }

  private getBodyMandatesUrl(bodyId) {
    return `${this.configurationService.apiUrl}/v1/bodies/${bodyId}/mandates`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toBodyMandate(res: Response): BodyMandate {
    let body = res.json();
    return body || {} as BodyMandate;
  }

  private toBodyMandates(res: Response): PagedResult<BodyMandateListItem> {
    return new PagedResultFactory<BodyMandateListItem>().create(res.headers, res.json());
  }
}
