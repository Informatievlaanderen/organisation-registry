import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  DelegationAssignmentFilter,
  DelegationAssignmentListItem,
  DelegationAssignment,
  CreateDelegationAssignmentRequest,
  UpdateDelegationAssignmentRequest,
  DeleteDelegationAssignmentRequest
} from './';

@Injectable()
export class DelegationAssignmentService {
  private delegationsUrl = `${this.configurationService.apiUrl}/v1/manage/delegations`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getDelegationAssignments(
    bodyMandateId: string,
    filter: DelegationAssignmentFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<DelegationAssignmentListItem>> {

    const url = `${this.delegationsUrl}/${bodyMandateId}/assignments`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toDelegationAssignments);
  }

  public get(bodyMandateId: string, id: string): Observable<DelegationAssignment> {
    const url = `${this.delegationsUrl}/${bodyMandateId}/assignments/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toDelegationAssignment);
  }

  public search(bodyMandateId: string, filter: DelegationAssignmentFilter): Observable<PagedResult<DelegationAssignmentListItem>> {
    const url = `${this.delegationsUrl}/${bodyMandateId}/assignments`;

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toDelegationAssignments);
  }

  public getAllDelegationAssignments(bodyMandateId: string): Observable<DelegationAssignmentListItem[]> {
    const url = `${this.delegationsUrl}/${bodyMandateId}/assignments`;

    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('personName', SortOrder.Ascending)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toDelegationAssignments)
      .map(pagedResult => pagedResult.data);
  }

  public create(bodyMandateId: string, aDelegation: CreateDelegationAssignmentRequest): Observable<string> {
    const url = `${this.delegationsUrl}/${bodyMandateId}/assignments`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(url, JSON.stringify(aDelegation), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(bodyMandateId: string, aDelegation: UpdateDelegationAssignmentRequest): Observable<string> {
    const url = `${this.delegationsUrl}/${bodyMandateId}/assignments/${aDelegation.delegationAssignmentId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(aDelegation), { headers: headers })
      .map(this.getLocationHeader);
  }

  public delete(bodyMandateId: string, aDelegation: DeleteDelegationAssignmentRequest): Observable<string> {
    const url = `${this.delegationsUrl}/${bodyMandateId}/assignments/${aDelegation.delegationAssignmentId}/${aDelegation.bodyId}/${aDelegation.bodySeatId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .delete(url, { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toDelegationAssignment(res: Response): DelegationAssignment {
    let body = res.json();
    return body || {} as DelegationAssignment;
  }

  private toDelegationAssignments(res: Response): PagedResult<DelegationAssignmentListItem> {
    return new PagedResultFactory<DelegationAssignmentListItem>().create(res.headers, res.json());
  }
}
