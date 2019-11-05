import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';
import { PagedResult, PagedResultFactory, SortOrder } from 'core/pagination';
import { ICrudService } from 'core/crud';

import { OrganisationBuildingListItem } from './organisation-building-list-item.model';
import { OrganisationBuilding } from './organisation-building.model';
import { OrganisationBuildingFilter } from './organisation-building-filter.model';

import { CreateOrganisationBuildingRequest, UpdateOrganisationBuildingRequest } from './';

@Injectable()
export class OrganisationBuildingService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getOrganisationBuildings(
    organisationId: string,
    filter: OrganisationBuildingFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize): Observable<PagedResult<OrganisationBuildingListItem>> {

    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationBuildingsUrl(organisationId), { headers: headers })
      .map(this.toOrganisationBuildings);
  }

  public get(organisationId: string, organisationBuildingId: string): Observable<OrganisationBuilding> {
    const url = `${this.getOrganisationBuildingsUrl(organisationId)}/${organisationBuildingId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationBuilding);
  }

  public create(organisationId, organisationBuilding: CreateOrganisationBuildingRequest): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.getOrganisationBuildingsUrl(organisationId), JSON.stringify(organisationBuilding), { headers: headers })
      .map(this.getLocationHeader);
  }

  public update(organisationId, organisationBuilding: UpdateOrganisationBuildingRequest): Observable<boolean> {
    const url = `${this.getOrganisationBuildingsUrl(organisationId)}/${organisationBuilding.buildingId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisationBuilding), { headers: headers })
      .map(response => response.ok);
  }

  private getOrganisationBuildingsUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/buildings`;
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisationBuilding(res: Response): OrganisationBuilding {
    let body = res.json();
    return body || {} as OrganisationBuilding;
  }

  private toOrganisationBuildings(res: Response): PagedResult<OrganisationBuildingListItem> {
    return new PagedResultFactory<OrganisationBuildingListItem>().create(res.headers, res.json());
  }
}
