import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  FormalFrameworkCategoryFilter,
  FormalFrameworkCategoryListItem,
  FormalFrameworkCategory
} from './';

@Injectable()
export class FormalFrameworkCategoryService implements ICrudService<FormalFrameworkCategory> {
  private formalFrameworkCategoriesUrl = `${this.configurationService.apiUrl}/v1/formalframeworkcategories`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getFormalFrameworkCategories(
    filter: FormalFrameworkCategoryFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<FormalFrameworkCategoryListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.formalFrameworkCategoriesUrl, { headers: headers })
      .map(this.toFormalFrameworkCategories);
  }

  public get(id: string): Observable<FormalFrameworkCategory> {
    const url = `${this.formalFrameworkCategoriesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toFormalFrameworkCategory);
  }

  public search(filter: FormalFrameworkCategoryFilter): Observable<PagedResult<FormalFrameworkCategoryListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.formalFrameworkCategoriesUrl, { headers: headers })
      .map(this.toFormalFrameworkCategories);
  }

  public getAllFormalFrameworkCategories(): Observable<FormalFrameworkCategoryListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.formalFrameworkCategoriesUrl, { headers: headers })
      .map(this.toFormalFrameworkCategories)
      .map(pagedResult => pagedResult.data);
  }

  public create(formalFrameworkCategory: FormalFrameworkCategory): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.formalFrameworkCategoriesUrl, JSON.stringify(formalFrameworkCategory), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(formalFrameworkCategory: FormalFrameworkCategory): Observable<string> {
    const url = `${this.formalFrameworkCategoriesUrl}/${formalFrameworkCategory.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(formalFrameworkCategory), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toFormalFrameworkCategory(res: Response): FormalFrameworkCategory {
    let body = res.json();
    return body || {} as FormalFrameworkCategory;
  }

  private toFormalFrameworkCategories(res: Response): PagedResult<FormalFrameworkCategoryListItem> {
    return new PagedResultFactory<FormalFrameworkCategoryListItem>().create(res.headers, res.json());
  }
}
