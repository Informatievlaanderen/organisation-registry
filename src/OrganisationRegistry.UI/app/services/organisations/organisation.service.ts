import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import {
  OrganisationFilter,
  Organisation,
  OrganisationChild,
  OrganisationListItem,
  ICreateOrganisation
} from './';
import { KboOrganisation } from './organisation.model';

@Injectable()
export class OrganisationService implements ICrudService<Organisation> {
  private organisationsUrl = `${this.configurationService.apiUrl}/v1/organisations`;
  private kboUrl = `${this.configurationService.apiUrl}/v1/kbo`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisations(
    filter: OrganisationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.organisationsUrl, { headers: headers })
      .map(this.toOrganisations);
  }

  public exportCsv(
    filter: OrganisationFilter,
    sortBy: string,
    sortOrder: SortOrder): Observable<string> {

    let headers = new HeadersBuilder()
      .csv()
      .withFiltering(filter)
      .withoutPagination()
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.organisationsUrl, { headers: headers })
      .map(r => r.text());
  }

  public search(filter: OrganisationFilter): Observable<PagedResult<OrganisationListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(1, this.configurationService.boundedPageSize)
      .build();

    return this.http
      .get(this.organisationsUrl, { headers: headers })
      .map(this.toOrganisations);
  }

  public get(id: string): Observable<Organisation> {
    const url = `${this.organisationsUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisation);
  }

  public getChildren(
    id: string,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationChild>> {
    const url = `${this.organisationsUrl}/${id}/children`;

    let headers = new HeadersBuilder()
      .json()
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationChildren);
  }

  public create(organisation: ICreateOrganisation): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.organisationsUrl, JSON.stringify(organisation), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisation: Organisation): Observable<string> {
    const url = `${this.organisationsUrl}/${organisation.id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisation), { headers: headers })
      .map(this.getLocationHeader);
  }

  public checkKbo(kbo: string): Observable<KboOrganisation> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    let result =
      this.http
        .get(`${this.kboUrl}/${kbo}?noRedirect=1`, { headers: headers });
    return result
      .map(this.toKboOrganisation);
  }

  public putKboNumber(organisationId: string, kboNumber: string) {
    const url = `${this.organisationsUrl}/${organisationId}/kbo/number/${kboNumber}`;

    let headers = new HeadersBuilder().build();

    return this.http
      .put(url, '', { headers: headers });
  }

  public terminate(organisationId: string, dateOfTermination: string) {
    const url = `${this.organisationsUrl}/${organisationId}/terminate`;

    let headers = new HeadersBuilder().build();

    return this.http
      .put(url, { dateOfTermination: dateOfTermination }, { headers: headers });
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisation(res: Response): Organisation {
    let body = res.json();
    return (body || {}) as Organisation;
  }

  private toKboOrganisation(res: Response): KboOrganisation {
    let body = res.json();
    return body || {} as KboOrganisation;
  }

  private toOrganisationChildren(res: Response): PagedResult<OrganisationChild> {
    return new PagedResultFactory<OrganisationChild>().create(res.headers, res.json());
  }

  private toOrganisations(res: Response): PagedResult<OrganisationListItem> {
    return new PagedResultFactory<OrganisationListItem>().create(res.headers, res.json());
  }
}
