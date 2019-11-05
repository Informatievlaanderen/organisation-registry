import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  LabelTypeFilter,
  LabelTypeListItem,
  LabelType
} from './';

@Injectable()
export class LabelTypeService implements ICrudService<LabelType> {
  private labelTypesUrl = `${this.configurationService.apiUrl}/v1/labeltypes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getLabelTypes(
    filter: LabelTypeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<LabelTypeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.labelTypesUrl, { headers: headers })
      .map(this.toLabelTypes);
  }

  public get(id: string): Observable<LabelType> {
    const url = `${this.labelTypesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toLabelType);
  }

  public search(filter: LabelTypeFilter): Observable<PagedResult<LabelTypeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.labelTypesUrl, { headers: headers })
      .map(this.toLabelTypes);
  }

  public getAllUserPermittedLabelTypes(): Observable<LabelTypeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.labelTypesUrl, { headers: headers })
      .map(this.toLabelTypes)
      .map(pagedResult => pagedResult.data.filter(x => x.userPermitted))
  }

  public create(labelType: LabelType): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.labelTypesUrl, JSON.stringify(labelType), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(labelType: LabelType): Observable<string> {
    const url = `${this.labelTypesUrl}/${labelType.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(labelType), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toLabelType(res: Response): LabelType {
    let body = res.json();
    return body || {} as LabelType;
  }

  private toLabelTypes(res: Response): PagedResult<LabelTypeListItem> {
    return new PagedResultFactory<LabelTypeListItem>().create(res.headers, res.json());
  }
}
