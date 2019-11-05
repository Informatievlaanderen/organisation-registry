import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { BodyLifecyclePhaseListItem } from './body-lifecycle-phase-list-item.model';
import { BodyLifecyclePhase } from './body-lifecycle-phase.model';

import { CreateBodyLifecyclePhaseRequest, UpdateBodyLifecyclePhaseRequest } from './';

@Injectable()
export class BodyLifecyclePhaseService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getBodyLifecyclePhases(
    bodyId: string,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BodyLifecyclePhaseListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getBodyLifecyclePhasesUrl(bodyId), { headers: headers })
      .map(this.toBodyLifecyclePhases);
  }

  public get(bodyId: string, bodyLifecyclePhaseId: string): Observable<BodyLifecyclePhase> {
    const url = `${this.getBodyLifecyclePhasesUrl(bodyId)}/${bodyLifecyclePhaseId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyLifecyclePhase);
  }

  public create(bodyId, bodyLifecyclePhase: CreateBodyLifecyclePhaseRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getBodyLifecyclePhasesUrl(bodyId), JSON.stringify(bodyLifecyclePhase), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(bodyId, bodyLifecyclePhase: UpdateBodyLifecyclePhaseRequest): Observable<boolean> {
    const url = `${this.getBodyLifecyclePhasesUrl(bodyId)}/${bodyLifecyclePhase.bodyLifecyclePhaseId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(bodyLifecyclePhase), { headers: headers })
      .map(response => response.ok);
  }

  private getBodyLifecyclePhasesUrl(bodyId) {
    return `${this.configurationService.apiUrl}/v1/bodies/${bodyId}/lifecyclephases`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toBodyLifecyclePhase(res: Response): BodyLifecyclePhase {
    let body = res.json();
    return body || {} as BodyLifecyclePhase;
  }

  private toBodyLifecyclePhases(res: Response): PagedResult<BodyLifecyclePhaseListItem> {
    return new PagedResultFactory<BodyLifecyclePhaseListItem>().create(res.headers, res.json());
  }
}
