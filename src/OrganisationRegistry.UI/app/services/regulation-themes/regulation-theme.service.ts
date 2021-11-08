import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  RegulationThemeFilter,
  RegulationThemeListItem,
  RegulationTheme
} from './';

@Injectable()
export class RegulationThemeService implements ICrudService<RegulationTheme> {
  private regulationThemesUrl = `${this.configurationService.apiUrl}/v1/regulationthemes`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getRegulationThemes(
    filter: RegulationThemeFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<RegulationThemeListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.regulationThemesUrl, { headers: headers })
      .map(this.toRegulationThemes);
  }

  public get(id: string): Observable<RegulationTheme> {
    const url = `${this.regulationThemesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toRegulationTheme);
  }

  public search(filter: RegulationThemeFilter): Observable<PagedResult<RegulationThemeListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.regulationThemesUrl, { headers: headers })
      .map(this.toRegulationThemes);
  }

  public getAllRegulationThemes(): Observable<RegulationThemeListItem[]> {
    let headers = new HeadersBuilder()
      .json()
      .withoutPagination()
      .withSorting('name', SortOrder.Ascending)
      .build();

    return this.http
      .get(this.regulationThemesUrl, { headers: headers })
      .map(this.toRegulationThemes)
      .map(pagedResult => pagedResult.data);
  }

  public create(regulationTheme: RegulationTheme): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.regulationThemesUrl, JSON.stringify(regulationTheme), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(regulationTheme: RegulationTheme): Observable<string> {
    const url = `${this.regulationThemesUrl}/${regulationTheme.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(regulationTheme), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toRegulationTheme(res: Response): RegulationTheme {
    let body = res.json();
    return body || {} as RegulationTheme;
  }

  private toRegulationThemes(res: Response): PagedResult<RegulationThemeListItem> {
    return new PagedResultFactory<RegulationThemeListItem>().create(res.headers, res.json());
  }
}
