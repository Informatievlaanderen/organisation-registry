import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  RegulationSubThemeFilter,
  RegulationSubThemeListItem,
  RegulationSubTheme
} from './';

@Injectable()
export class RegulationSubThemeService implements ICrudService<RegulationSubTheme> {
  private regulationSubThemesUrl = `${this.configurationService.apiUrl}/v1/regulationsubthemes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getRegulationSubThemes(
    filter: RegulationSubThemeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<RegulationSubThemeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.regulationSubThemesUrl, { headers: headers })
      .map(this.toRegulationSubThemes);
  }

  public get(id: string): Observable<RegulationSubTheme> {
    const url = `${this.regulationSubThemesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toRegulationSubTheme);
  }

  public search(filter: RegulationSubThemeFilter): Observable<PagedResult<RegulationSubThemeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.regulationSubThemesUrl, { headers: headers })
      .map(this.toRegulationSubThemes);
  }

  public getAllRegulationSubThemes(regulationThemeId: string): Observable<RegulationSubThemeListItem[]> {
    let search = new RegulationSubThemeFilter();
    search.regulationThemeId = regulationThemeId;


    let headers = new HeadersBuilder()
      .json()
      .withFiltering(search)
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.regulationSubThemesUrl, { headers: headers })
      .map(this.toRegulationSubThemes)
      .map(pagedResult => pagedResult.data);
  }

  public create(regulationSubTheme: RegulationSubTheme): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.regulationSubThemesUrl, JSON.stringify(regulationSubTheme), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(regulationSubTheme: RegulationSubTheme): Observable<string> {
    const url = `${this.regulationSubThemesUrl}/${regulationSubTheme.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(regulationSubTheme), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toRegulationSubTheme(res: Response): RegulationSubTheme {
    let body = res.json();
    return body || {} as RegulationSubTheme;
  }

  private toRegulationSubThemes(res: Response): PagedResult<RegulationSubThemeListItem> {
    return new PagedResultFactory<RegulationSubThemeListItem>().create(res.headers, res.json());
  }
}
