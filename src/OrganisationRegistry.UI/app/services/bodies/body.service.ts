import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  BodyFilter,
  Body,
  BodyValidity,
  BodyInfo,
  BodyBalancedParticipation,
  BodyListItem,
  ICreateBody
} from './';

@Injectable()
export class BodyService implements ICrudService<Body> {
  private bodiesUrl = `${this.configurationService.apiUrl}/v1/bodies`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getBodies(
    filter: BodyFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<BodyListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.bodiesUrl, { headers: headers })
      .map(this.toBodies);
  }

  public exportCsv(
    filter: BodyFilter,
    sortBy: string,
    sortOrder: SortOrder): Observable<string> {

    let headers = new HeadersBuilder()
      .csv()
      .withFiltering(filter)
      .withoutPagination()
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.bodiesUrl, { headers: headers })
      .map(r => r.text());
  }

  public search(filter: BodyFilter): Observable<PagedResult<BodyListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.bodiesUrl, { headers: headers })
      .map(this.toBodies);
  }

  public get(id: string): Observable<Body> {
    const url = `${this.bodiesUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBody);
  }

  public create(body: ICreateBody): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.bodiesUrl, JSON.stringify(body), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(body: Body): Observable<string> {
    const url = `${this.bodiesUrl}/${body.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(body), { headers: headers })
      .map(this.getLocationHeader);
  }

  public getValidity(id: string): Observable<BodyValidity> {
    const url = `${this.bodiesUrl}/${id}/validity`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyValidity);
  }

  public changeValidity(body: BodyValidity): Observable<string> {
    const url = `${this.bodiesUrl}/${body.id}/validity`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(body), { headers: headers })
      .map(this.getLocationHeader);
  }

  public getInfo(id: string): Observable<BodyInfo> {
    const url = `${this.bodiesUrl}/${id}/info`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyInfo);
  }

  public changeInfo(body: BodyInfo): Observable<string> {
    const url = `${this.bodiesUrl}/${body.id}/info`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(body), { headers: headers })
      .map(this.getLocationHeader);
  }

  public getBalancedParticipation(id: string): Observable<BodyBalancedParticipation> {
    const url = `${this.bodiesUrl}/${id}/balancedparticipation`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toBodyBalancedParticipation);
  }

  public changeBalancedParticipation(body: BodyBalancedParticipation): Observable<string> {
    const url = `${this.bodiesUrl}/${body.id}/balancedparticipation`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(body), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toBody(res: Response): Body {
    let body = res.json();
    return (body || {}) as Body;
  }

  private toBodyValidity(res: Response): BodyValidity {
    let bodyValidity = res.json();
    return (bodyValidity || {}) as BodyValidity;
  }

  private toBodyInfo(res: Response): BodyInfo {
    let bodyInfo = res.json();
    return (bodyInfo || {}) as BodyInfo;
  }

  private toBodyBalancedParticipation(res: Response): BodyBalancedParticipation {
    let bodyBalancedParticipation = res.json();
    return new BodyBalancedParticipation(bodyBalancedParticipation || {});
  }

  private toBodies(res: Response): PagedResult<BodyListItem> {
    return new PagedResultFactory<BodyListItem>().create(res.headers, res.json());
  }
}
