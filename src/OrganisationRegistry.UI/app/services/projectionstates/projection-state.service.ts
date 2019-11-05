import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
    ProjectionState,
    ProjectionStateFilter,
    ProjectionStateListItem    
} from './';

@Injectable()
export class ProjectionStateService implements ICrudService<ProjectionState> {
  private projectionsUrl = `${this.configurationService.apiUrl}/v1/projections`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getProjectionStates(
    filter: ProjectionStateFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<ProjectionStateListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(`${this.projectionsUrl}/states/`, { headers: headers })
      .map(this.toProjectionStates);
  }

  public get(id: string): Observable<ProjectionState> {
    const url = `${this.projectionsUrl}/states/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toProjectionState);
  }

  public create(state: ProjectionState): Observable<string> {
    return;
  }

  public update(state: ProjectionState): Observable<string> {
    const url = `${this.projectionsUrl}/states/${state.id}`;
    
        let headers = new HeadersBuilder()
          .json()
          .build();
    
        return this.http
          .put(url, JSON.stringify(state), { headers: headers })
          .map(this.getLocationHeader);    
  }

  public getLastEventNumber(): Observable<number> {

    const url = `${this.projectionsUrl}/states/last-event`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(res => res.json() || 0 as number);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toProjectionState(res: Response): ProjectionState {
    let body = res.json();
    return body || {} as ProjectionState;
  }

  private toProjectionStates(res: Response): PagedResult<ProjectionStateListItem> {
    return new PagedResultFactory<ProjectionStateListItem>().create(res.headers, res.json());
  }
}
